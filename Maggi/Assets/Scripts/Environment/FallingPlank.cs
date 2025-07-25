using UnityEngine;
using UnityEngine.Serialization;

public class FallingPlank : MonoBehaviour
{
    [SerializeField] private string _floorLayerName = "Floor";  // Inspector에서 레이어 이름 지정
    [SerializeField] private float _delay = 1.0f;
    
    private Rigidbody _rb;
    private bool _hasLanded;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = false;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rb.mass = 50;
    }

    private void OnCollisionEnter(Collision other)
    {
        int floorLayer = LayerMask.NameToLayer(_floorLayerName);
        if (!_hasLanded && other.gameObject.layer == floorLayer)
        {
            _hasLanded = true;
            _rb.isKinematic = true;
        }
    }
}