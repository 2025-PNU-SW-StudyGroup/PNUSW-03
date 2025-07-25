using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public struct ItascIkJob : IWeightedAnimationJob
{
    public NativeArray<ReadWriteTransformHandle> boneHandles;
    public ReadWriteTransformHandle tipHandle;
    public ReadOnlyTransformHandle targetHandle;
    public ReadOnlyTransformHandle hintHandle;
    public Quaternion tipRotationOffset;
    public int iterations;
    public float stepWeight;
    public float tolerance;

    // --- IWeightedAnimationJob 필수 멤버 ---
    public FloatProperty jobWeight { get; set; }

    public void ProcessRootMotion(AnimationStream stream) { }

    public void ProcessAnimation(AnimationStream stream)
    {
        float w = jobWeight.Get(stream);
        if (w <= 0f || !targetHandle.IsValid(stream) || !boneHandles.IsCreated || boneHandles.Length == 0)
        {
            return;
        }

        // --- 1. 힌트(Pole Vector)를 이용한 방향 설정 (가장 먼저 수행) ---
        if (hintHandle.IsValid(stream) && boneHandles.Length >= 3) // 3개 이상의 뼈 체인에 적용
        {
            var rootHandle = boneHandles[0];
            var midHandle = boneHandles[boneHandles.Length / 2]; // 체인의 중간 뼈를 맞춤치료 간주
            var tipHandle = boneHandles[boneHandles.Length - 1];

            Vector3 rootPos = rootHandle.GetPosition(stream);
            Vector3 midPos = midHandle.GetPosition(stream);
            Vector3 tipPos = tipHandle.GetPosition(stream);
            Vector3 hintPos = hintHandle.GetPosition(stream);

            Vector3 currentPlaneNormal = Vector3.Cross(midPos - rootPos, tipPos - rootPos);
            Vector3 targetPlaneNormal = Vector3.Cross(hintPos - rootPos, tipPos - rootPos);

            if (currentPlaneNormal.sqrMagnitude > 0.0001f && targetPlaneNormal.sqrMagnitude > 0.0001f)
            {
                Quaternion correctionRotation = Quaternion.FromToRotation(currentPlaneNormal.normalized, targetPlaneNormal.normalized);
                rootHandle.SetRotation(stream, Quaternion.Slerp(rootHandle.GetRotation(stream), correctionRotation * rootHandle.GetRotation(stream), w));
            }
            //Debug.Log("힌트 조정 수행");
        }

        // --- 2. ITASC 알고리즘으로 위치 맞추기 (나중에 수정) ---
        Vector3 targetPosition = targetHandle.GetPosition(stream);
        iterations = 15;

        for (int i = 0; i < iterations; i++)
        {
            // 매 반복마다 현재 Tip의 위치를 가져옵니다.
            Vector3 tipPosition = this.tipHandle.GetPosition(stream);

            // 목표에 도달했는지 확인
            if (Vector3.SqrMagnitude(tipPosition - targetPosition) < tolerance * tolerance)
                break;

            // Tip에 가까운 뼈부터 역순으로 순회합니다.
            for (int j = boneHandles.Length - 1; j >= 0; j--)
            {
                var boneHandle = boneHandles[j];
                Vector3 bonePosition = boneHandle.GetPosition(stream);

                // --- 이 부분이 핵심적인 수정 ---
                // tipPosition은 매번 새로 가져와야 합니다.
                tipPosition = this.tipHandle.GetPosition(stream);

                // 현재 뼈에서 팔 끝(Tip)까지의 벡터
                Vector3 toEffector = tipPosition - bonePosition;
                // 현재 뼈에서 목표(Target)까지의 벡터
                Vector3 toTarget = targetPosition - bonePosition;

                // 두 벡터 사이의 회전을 계산합니다.
                Quaternion deltaRotation = Quaternion.FromToRotation(toEffector.normalized, toTarget.normalized);

                // 계산된 회전을 현재 뼈의 회전에 적용합니다.
                // stepWeight와 w를 곱하여 움직임을 제어합니다.
                Quaternion currentRotation = boneHandle.GetRotation(stream);
                boneHandle.SetRotation(
                    stream,
                    Quaternion.Slerp(currentRotation, deltaRotation * currentRotation, stepWeight * w)
                );
            }
        }

        
        Quaternion finalTargetRotation = targetHandle.GetRotation(stream) * tipRotationOffset;

        // Quaternion.Slerp를 사용하여 현재 Tip의 회전에서 최종 목표 회전으로 부드럽게 보간합니다.
        tipHandle.SetRotation(
            stream,
            Quaternion.Slerp(
                tipHandle.GetRotation(stream),
                finalTargetRotation,
                w // 전체 IK 가중치 적용
            )
        );
    }
}