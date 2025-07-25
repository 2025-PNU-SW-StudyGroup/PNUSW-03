using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using Unity.Collections;
using System.Collections.Generic;

public class ItascIkJobBinder : AnimationJobBinder<ItascIkJob, ItascIkJobData>
{
    public override ItascIkJob Create(Animator animator, ref ItascIkJobData data, Component component)
    {
        var job = new ItascIkJob();
        var bones = GetBoneChain(data.root, data.tip);
        job.boneHandles = new NativeArray<ReadWriteTransformHandle>(bones.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        for (int i = 0; i < bones.Length; i++)
        {
            job.boneHandles[i] = ReadWriteTransformHandle.Bind(animator, bones[i]);
        }

        job.tipHandle = ReadWriteTransformHandle.Bind(animator, data.tip);
        job.targetHandle = ReadOnlyTransformHandle.Bind(animator, data.target);

        if (data.hint != null)
            job.hintHandle = ReadOnlyTransformHandle.Bind(animator, data.hint);
        else
            job.hintHandle = default(ReadOnlyTransformHandle);

        // --- 이 부분을 추가합니다 ---
        // 초기 회전 오프셋을 계산하여 Job에 저장합니다.
        if (data.target != null && data.tip != null)
        {
            job.tipRotationOffset = Quaternion.Inverse(data.target.rotation) * data.tip.rotation;
        }
        else
        {
            job.tipRotationOffset = Quaternion.identity;
        }
        // ------------------------

        return job;
    }

    public override void Destroy(ItascIkJob job)
    {
        // Create에서 할당한 NativeArray를 여기서 해제합니다.
        if (job.boneHandles.IsCreated)
            job.boneHandles.Dispose();
    }

    // Update는 RigConstraint 베이스 클래스가 weight를 자동으로 처리하므로,
    // 다른 값들을 실시간으로 바꾸고 싶지 않다면 비워둬도 됩니다.
    // 만약 실시간 변경이 필요하다면 아래 코드를 사용합니다.
    public override void Update(ItascIkJob job, ref ItascIkJobData data)
    {
        job.iterations = data.iterations;
        job.stepWeight = data.stepWeight;
        job.tolerance = data.tolerance;
    }

    private static Transform[] GetBoneChain(Transform root, Transform tip)
    {
        if (root == null || tip == null) return new Transform[0];
        var chain = new List<Transform>();
        var current = tip;
        while (current != null && current != root.parent)
        {
            chain.Add(current);
            if (current == root) break;
            current = current.parent;
        }
        chain.Reverse();
        return chain.ToArray();
    }
}