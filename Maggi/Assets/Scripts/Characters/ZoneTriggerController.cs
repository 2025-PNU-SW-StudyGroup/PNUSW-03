using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BoolEvent : UnityEvent<bool, GameObject> { } 

public class ZoneTriggerController : MonoBehaviour
{
    [SerializeField] protected BoolEvent _enterZone = default;
    [SerializeField] protected LayerMask _layers = default;
    [SerializeField] private const float _cooldownTime = 0.8f; // 쿨타임
    
    private float _lastTriggerTime; // 마지막으로 호출된 시간을 저장할 변수

    private void OnTriggerEnter(Collider other)
    {
        // _layers는 여러 layer를 복수 선택할 수 있기 때문에 아래처럼 Bit 연산한다.
        if ((1 << other.gameObject.layer & _layers) != 0)
        {
            _enterZone.Invoke(true, other.gameObject);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        // 현재 시간이 마지막 호출 시간 + 쿨타임보다 큰 경우에만 실행
        if (Time.time >= _lastTriggerTime + _cooldownTime)
        {
            if ((1 << other.gameObject.layer & _layers) != 0)
            {
                _enterZone.Invoke(true, other.gameObject);
        
                // 마지막 호출 시간을 현재 시간으로 갱신
                _lastTriggerTime = Time.time; 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((1 << other.gameObject.layer & _layers) != 0)
        {   
            _enterZone.Invoke(false, other.gameObject);
        }
    }
}
