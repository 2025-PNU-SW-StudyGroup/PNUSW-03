using UnityEngine;

namespace AmazingAssets.CurvedWorld.Examples
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;         // 따라갈 타겟
        public float smoothing = 5f;     // 따라가는 속도
        public float zOffsetAdjust = 0f; // Z축으로 얼마나 더 가까이/멀리 당길지

        Vector3 offset;  // 초기 offset

        void Start()
        {
            // 초기 offset 계산
            offset = transform.position - target.position;

            // Z축 거리 보정
            offset.z += zOffsetAdjust;
        }

        void LateUpdate()
        {
            // 목표 위치 = 타겟 위치 + 수정된 offset
            Vector3 targetCamPos = target.position + offset;

            // 부드럽게 이동
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
    }
}
