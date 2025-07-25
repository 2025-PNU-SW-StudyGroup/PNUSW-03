using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    [SerializeField] private TransformAnchor _playerTransformAnchor;
    [SerializeField] private TransformAnchor _cameraTransformAnchor;
    [SerializeField] private CameraSO _currentCamera;

    [Header("Listening to")]
    [SerializeField] private VoidEventChannelSO _onSwitchCamera;
    [SerializeField] private VoidEventChannelSO _cameraShakeEvent;

    private float _edgeThreshold = 5f; // 마우스로 Aim 조절하기 하는 가장자리의 pixel 수
    private Vector2 _preDir =  Vector2.zero; // 화살표키 동작을 막지 않기 위해 사용하는 flag

    // Virtual Camara들을 Inspector에서 순서대로 가져온다. ! 순서 유의 중요 !
    private CinemachineVirtualCamera[] _virtualCams;

    public bool lockCamera;

    private void OnEnable()
    {
        _inputReader.AimArrowEvent += Aim;
        _inputReader.AimMouseEvent += AimMouse;
        _playerTransformAnchor.OnAnchorProvided += SetupPlayerVirtualCamera;
        _onSwitchCamera.OnEventRaised += SwitchToCamera;
        _cameraShakeEvent.OnEventRaised += _impulseSource.GenerateImpulse;

        _cameraTransformAnchor.Provide(_mainCamera.transform);
    }

    private void OnDisable()
    {
        _inputReader.AimArrowEvent -= Aim;
        _playerTransformAnchor.OnAnchorProvided -= SetupPlayerVirtualCamera;
        _onSwitchCamera.OnEventRaised -= SwitchToCamera;
        _cameraShakeEvent.OnEventRaised -= _impulseSource.GenerateImpulse;
    }

    private void Start()
    {
        _virtualCams = GetComponentsInChildren<CinemachineVirtualCamera>();
        if (_virtualCams.Length == 0)
            Debug.LogWarning("There is no virtual camera _ CameraManager.cs");

        if (_playerTransformAnchor.isSet)
            SetupPlayerVirtualCamera();
        
        Aim(Vector2.zero);
    }
    
#if UNITY_EDITOR
    private void Update()
    {
        // 마우스가 어디를 가리키는지 디버깅용
        //DrawRayMousePosition();
        
        if (!_inputReader.GetGameplayInput())
            Aim(Vector2.zero);
    }
#endif
    
    private void AimMouse(Vector2 m)
    {
        if (lockCamera) return;
        
        Vector2 dir = Vector2.zero;

        // left / right
        if (m.x <= _edgeThreshold)                
            dir.x = -1f;
        else if (m.x >= Screen.width - _edgeThreshold)  
            dir.x = +1f;

        // bottom / top
        if (m.y <= _edgeThreshold)                
            dir.y = -1f;
        else if (m.y >= Screen.height - _edgeThreshold) 
            dir.y = +1f;

        // 정상 화면으로 돌리기 위해 pre direction을 가져와 확인
        if (_preDir != Vector2.zero)
        {
            Aim(dir.normalized);
        }
        
        _preDir = dir;
    }

    private void SetupPlayerVirtualCamera()
    {
        Transform target = _playerTransformAnchor.Value;

        foreach (var virtualCam in _virtualCams)
        {
            virtualCam.Follow = target;
            virtualCam.LookAt = target;
            if (target != null)
                virtualCam.OnTargetObjectWarped(target, target.position - virtualCam.transform.position - Vector3.forward);
        }
    }

    private void SwitchToCamera()
    {
        if (_currentCamera.index < 0 || _currentCamera.index >= _virtualCams.Length)
            return;

        for (int i = 0; i < _virtualCams.Length; ++i)
        {
            _virtualCams[i].Priority = (i == _currentCamera.index) ? 1 : 0;
        }
        
        Aim(Vector2.zero);
    }

    private void Aim(Vector2 normalDirection)
    {
        if (_virtualCams[_currentCamera.index] != null)
        {
            CinemachineComposer composer = _virtualCams[_currentCamera.index].GetCinemachineComponent<CinemachineComposer>();

            // 스무스 하게 하자
            composer.m_ScreenX = normalDirection.x == 0 ? 0.5f : 0.5f - 0.4f * normalDirection.x;
            float yOffset = normalDirection.y >= 0 ? 0.3f : 0.6f;
            composer.m_ScreenY = normalDirection.y == 0 ? 0.65f : 0.65f + yOffset * normalDirection.y;
        }
    }

    /* Execute in Animation Clip */
    public void ShakeCamera()
    {
        _cameraShakeEvent.RaiseEvent();
    }
    
    private void DrawRayMousePosition()
    {
        // 현재 마우스 Ray가 어디를 쏘고 있는지 Debugging 용
        
        // 1. Screen → Ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // 2. Ray 시각화 (100 유닛 길이로 그리기)
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        Debug.Log("Ray 시각화");
        
        // 3. 실제 충돌 지점
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            Debug.Log($"Ray hit: {hit.collider.name} at {hit.point}");
        }
    }
}
