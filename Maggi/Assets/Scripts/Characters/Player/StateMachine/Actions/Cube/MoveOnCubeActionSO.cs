using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "MoveOnCubeAction", menuName = "State Machines/Actions/Move On Cube Action")]
public class MoveOnCubeActionSO : StateActionSO
{
    public float moveSpeed = 3f;             // 면 위에서의 이동 속도
    public float edgeTransitionTime = 0.25f; // 모서리 넘어갈 때 부드럽게 회전하는 데 걸리는 시간
    public float turnSmoothTime = 5.0f;

    protected override StateAction CreateAction() => new MoveOnCubeAction();
}

public class MoveOnCubeAction : StateAction
{
    private MoveOnCubeActionSO _originSO => (MoveOnCubeActionSO)base.OriginSO;

    private Player _player;
    private Transform _transform;
    private Transform _cubeTransform;   // 정육면체(또는 직사육면체)
    private BoxCollider _cubeCollider;
    private const float ROTATION_TRESHOLD = 0.02f;

    // 현재 서 있는 면의 노멀
    private Vector3 _currentFaceNormal;

    // -- 모서리 전환(Edge Transition) 관련 --
    private bool _isEdgeTransition;           // 모서리 전환 중인지
    private float _edgeTransitionElapsed;     // 전환 경과 시간
    private Quaternion _startRotation;        // 전환 시작 시 회전
    private Quaternion _endRotation;          // 전환 목표 회전
    private Vector3 _oldNormal;               // 전환 시작 시 면 노멀
    private Vector3 _newNormal;               // 전환 목표 면 노멀
    private Vector3 _edgeAxis;                // 회전 축(두 면 노멀의 외적)
    private float _edgeAngle;                 // 두 면 노멀 사이 각도(보통 90도)
    private Vector3 _playerLocalPos;          // 플레이어의 좌표를 Cube 로컬 공간 상의 좌표로 변환
    private Vector3 _cubeHalfSize;            // Cube의 Half size
    private Vector3 _localNormal;             // faceNormal (월드) → 큐브 로컬 노멀

    public override void Awake(InteractiveObject interactiveObject, GameObject owner)
    {
        _cubeTransform = interactiveObject.transform;
        _cubeCollider = interactiveObject.GetComponent<BoxCollider>();
        _player = owner.GetComponent<Player>();
        _transform = _player.transform;
    }

    public override void OnStateEnter()
    {
        if (!_cubeTransform)
        {
            Debug.LogWarning("Cube Transform not assigned!");
            return;
        }
        
        // 플레이어의 좌표를 Cube 로컬 공간 상의 좌표로 변환한다.
        // 이제는 Cube의 원점을 기준으로 플레이어 좌표를 계산한다.
        _playerLocalPos = _cubeTransform.InverseTransformPoint(_transform.position);
        _cubeHalfSize = Vector3.Scale(_cubeCollider.size, _cubeTransform.localScale) * 0.5f;
        // 초기 면 노멀
        _currentFaceNormal = GetClosestFaceNormal();
        _localNormal = _cubeTransform.InverseTransformDirection(_currentFaceNormal);
        // 면 표면에 붙이기
        StickToFace();
    }

    public override void OnUpdate()
    {
        // 1. (정상 이동) Cube가 없다면 중단
        if (!_cubeTransform)
            return;

        // 플레이어의 좌표를 Cube 로컬 공간 상의 좌표로 변환한다.
        // 이제는 Cube의 원점을 기준으로 플레이어 좌표를 계산한다.
        _playerLocalPos = _cubeTransform.InverseTransformPoint(_transform.position);
        _cubeHalfSize = Vector3.Scale(_cubeCollider.size, _cubeTransform.localScale) * 0.5f;
        
        // 2. 현재 면 판별 (ex. X/Y/Z 면 중 어디에 붙었는지)
        _currentFaceNormal = GetClosestFaceNormal();
        Debug.Log($"before current face normal = {_currentFaceNormal}");
        _localNormal = _cubeTransform.InverseTransformDirection(_currentFaceNormal);

        // 3. 이동 입력
        Vector2 input = _player.inputVector; // (x: 좌우, y: 전후)
        MoveOnFace(input);
        Debug.Log($"after current face normal = {_currentFaceNormal}");
    }
    
    /// <summary>
    /// 현재 위치에서 가장 가까운 면(±X/±Y/±Z) 노멀을 구함
    /// </summary>
    private Vector3 GetClosestFaceNormal()
    {
        // 비율이 클 수록 해당 축 방향 면에 더 가깝다
        float ratioX = Mathf.Abs(_playerLocalPos.x) / _cubeHalfSize.x;
        float ratioY = Mathf.Abs(_playerLocalPos.y) / _cubeHalfSize.y;
        float ratioZ = Mathf.Abs(_playerLocalPos.z) / _cubeHalfSize.z;

        // 가장 가까운 면이 X
        if (ratioX > ratioY && ratioX > ratioZ)
        {
            Debug.Log("가까운 면 X");
            // 로컬 +X 면 = Cube의 오른쪽 면
            if (_playerLocalPos.x >= 0) return _cubeTransform.TransformDirection(Vector3.right);
            // 로컬 -X 면 = Cube의 왼쪽 면
            return _cubeTransform.TransformDirection(Vector3.left);
        }
        // 가장 가까운 면이 Y면
        else if (ratioY > ratioX && ratioY > ratioZ)
        {
            Debug.Log("가까운 면 Y");
            // 로컬 +Y 면 = Cube의 위쪽 면
            if (_playerLocalPos.y >= 0) return _cubeTransform.TransformDirection(Vector3.up);
            // 로컬 -Y 면 = Cube의 아래쪽 면
            return _cubeTransform.TransformDirection(Vector3.down);
        }
        // 가장 가까운 면이 Z면
        else
        {
            Debug.Log("가까운 면 Z");
            // 로컬 +Z 면 = Cube의 앞쪽 면
            if (_playerLocalPos.z >= 0) return _cubeTransform.TransformDirection(Vector3.forward);
            // 로컬 -Z 면 = Cube의 뒷쪽 면
            return _cubeTransform.TransformDirection(Vector3.back);
        }
    }
    
    /// <summary>
    /// 면 위에서 이동 처리
    /// </summary>
    private void MoveOnFace(Vector2 input)
    {
        // "Forward = 면∩YZ 교선, Right = 면∩XY 교선"
        // 월드 좌표계를 기준으로 forward와 right의 Path를 결정한다.
        // 교선 = cross(면노멀, 평면노멀)
        ComputeFaceAxes(out Vector3 faceRight, out Vector3 faceForward);

        // 이동 벡터
        // forward => input.y, right => input.x
        Vector3 newMovementVector = (faceForward * input.y + faceRight * input.x).normalized * _originSO.moveSpeed;
        _player.movementVector = newMovementVector;

        // 면 표면에 붙이기
        StickToFace();

        // 플레이어 회전
        _transform.rotation = Quaternion.FromToRotation(_transform.up, _currentFaceNormal) * _transform.rotation;

        // player를 이동 벡터 방향으로 회전
        if (_player.movementVector.sqrMagnitude >= ROTATION_TRESHOLD)
        {
            Quaternion targetRotation = Quaternion.LookRotation(newMovementVector, _currentFaceNormal);
            // _transform.up을 기준으로 90도 회전
            Quaternion additionalRotation = Quaternion.AngleAxis(-90.0f, _transform.up);

            // 현재 회전에 추가 회전을 곱함
            targetRotation = additionalRotation * targetRotation;

            // 3) slerp
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, _originSO.turnSmoothTime * Time.deltaTime);
        }
    }

    /// <summary>
    /// "player"를 "cube"의 특정 면(faceNormal)에 정확히 붙이는 함수.
    ///  - cube가 회전/비균일 스케일이어도 올바르게 동작.
    ///  - player.localScale을 반지름/절반 크기로 사용.
    ///  - 입력: faceNormal은 월드 기준 면 노멀(큐브 회전·스케일 반영됨).
    /// </summary>
    private void StickToFace()
    {
        // 2) 플레이어 반절 크기(절반 Extent)
        //    - 여기서는 player.localScale / 2 로 간단히 처리.
        //    - 만약 플레이어 콜라이더가 Capsule인 경우, 정확히는 collider.height/2 등으로 바꿔야 함.
        Vector3 playerHalf = _transform.localScale * 0.5f;

        // 5) 로컬 노멀 방향(±X, ±Y, ±Z)에 따라 localPos 의 해당 축 값을 고정
        float dotX = Vector3.Dot(_localNormal, Vector3.right);
        float dotY = Vector3.Dot(_localNormal, Vector3.up);
        float dotZ = Vector3.Dot(_localNormal, Vector3.forward);

        // (절댓값이 0.99 이상 → 거의 해당 축 면에 붙어 있는 상황)
        
        // localNormal.x > 0 → 로컬 +X 면, localNormal.x < 0 → 로컬 -X 면
        if (Mathf.Abs(dotX) > 0.99f)
        {
            if (dotX > 0f)
            {
                // 로컬 +X 면 : x = +half.x + playerHalf.x
                _playerLocalPos.x = _cubeHalfSize.x + playerHalf.x;
            }
            else
            {
                // 로컬 -X 면 : x = -half.x - playerHalf.x
                _playerLocalPos.x = -_cubeHalfSize.x - playerHalf.x;
            }
        }
        else if (Mathf.Abs(dotY) > 0.99f)
        {
            if (dotY > 0f)
            {
                // 로컬 +Y 면 : y = +half.y + playerHalf.y
                _playerLocalPos.y = _cubeHalfSize.y + playerHalf.y;
            }
            else
            {
                // 로컬 -Y 면 : y = -half.y - playerHalf.y
                _playerLocalPos.y = -_cubeHalfSize.y - playerHalf.y;
            }
        }
        else
        {
            if (dotZ > 0f)
            {
                // 로컬 +Z 면 : z = +half.z + playerHalf.z
                _playerLocalPos.z = _cubeHalfSize.z + playerHalf.z;
            }
            else
            {
                // 로컬 -Z 면 : z = -half.z - playerHalf.z
                _playerLocalPos.z = -_cubeHalfSize.z - playerHalf.z;
            }
        }

        // 6) 수정된 큐브 로컬 좌표(localPos) → 월드 좌표로 변환
        Vector3 newWorldPos = _cubeTransform.TransformPoint(_playerLocalPos);

        // 7) 플레이어 위치 업데이트
        _transform.position = newWorldPos;
    }


    /// <summary>
    /// 면 노멀 faceNormal이 있을 때,
    /// 1) faceForward = (면∩YZ 교선) 을 '양의 방향'으로 보정
    /// 2) faceRight   = (면∩XY 교선) 을 '양의 방향'으로 보정
    /// </summary>
    private void ComputeFaceAxes(out Vector3 faceRight, out Vector3 faceForward)
    {
        // YZ 평면 normal => (1,0,0)
        Vector3 yzNormal = _cubeTransform.right;
        // XY 평면 normal => (0,0,1)
        Vector3 xyNormal = _cubeTransform.forward;

        // 1) 교선 계산
        //    - forward: 교선( faceNormal ∩ YZ ), = cross(faceNormal, yzNormal)
        //    - right:   교선( faceNormal ∩ XY ), = cross(faceNormal, xyNormal)
        faceForward = -Vector3.Cross(_currentFaceNormal, yzNormal);
        faceRight = Vector3.Cross(_currentFaceNormal, xyNormal);

        // 혹시 교선이 0벡터?
        if (faceForward.sqrMagnitude < 1e-6f) faceForward = Vector3.forward;
        else faceForward.Normalize();

        if (faceRight.sqrMagnitude < 1e-6f) faceRight = Vector3.right;
        else faceRight.Normalize();
    }
}
