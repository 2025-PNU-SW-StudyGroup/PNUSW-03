using UnityEngine;
using UnityEngine.Animations.Rigging;
using Unity.Collections;

[System.Serializable]
public struct ItascIkJobData : IAnimationJobData
{
    // --- 인스펙터에서 설정할 모든 변수 ---
    [SerializeField] public Transform root;
    [SerializeField] public Transform tip;
    [SerializeField] public Transform target;
    [SerializeField] public Transform hint;

    [SerializeField] public int iterations;
    [SerializeField][Range(0.01f, 1f)] public float stepWeight;
    [SerializeField] public float tolerance;

    // --- Job에서 사용할 데이터 (코드로 채워짐) ---
    [System.NonSerialized] public NativeArray<ReadWriteTransformHandle> boneHandles;
    [System.NonSerialized] public ReadWriteTransformHandle tipHandle;
    [System.NonSerialized] public ReadOnlyTransformHandle targetHandle;
    [System.NonSerialized] public ReadOnlyTransformHandle hintHandle;
    [System.NonSerialized] public Quaternion tipRotationOffset;

    public bool IsValid() => root != null && tip != null && target != null;

    public void SetDefaultValues()
    {
        root = null;
        tip = null;
        target = null;
        hint = null;
        iterations = 15;
        stepWeight = 0.5f;
        tolerance = 0.001f;
    }
}