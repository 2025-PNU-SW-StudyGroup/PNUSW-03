using UnityEngine;
using UnityEngine.Animations.Rigging;

[DisallowMultipleComponent]
[AddComponentMenu("Animation Rigging/Custom/iTaSC IK Constraint")]
public class ItascIkConstraint : RigConstraint<
    ItascIkJob,
    ItascIkJobData,
    ItascIkJobBinder
>
{
    // 이 클래스는 비어있어야 합니다.
    // 모든 데이터와 로직은 부모 클래스인 RigConstraint와
    // ItascIkJobData, ItascIkJobBinder에서 처리합니다.
}