using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "PullHeavyAction", menuName = "State Machines/Actions/Pull Heavy Action")]
public class PullHeavyActionSO : StateActionSO<PullHeavyAction>
{
    public LayerMask collisionLayerMask;
    public float pullStep = 0.05f; // 물체를 얼마나 조금씩 움직일지
    public float maxPullDistanceOffset = 0.2f;  // offset에 이 값을 더한 만큼만 움직일 수 있음
    public float lerpDuration = 0.2f;
}

public class PullHeavyAction : StateAction
{
    protected new PullHeavyActionSO _originSO => (PullHeavyActionSO)base.OriginSO;
    private Player _player;
    private CharacterController _characterController;
    private InteractionManager _interactionManager;
    private InteractiveObject _interactionObject;

    private Rigidbody _interactiveObjectRigidbody;
    private Vector3 _offset = Vector3.zero;
    private Vector3 _newPosition;
    private bool _isValidState = true;
    private int _defaultLayer = LayerMask.NameToLayer("Default");
    private int _wallLayer = LayerMask.NameToLayer("Wall");

    public override void Awake(StateMachine stateMachine)
    {
        _player = stateMachine.GetComponent<Player>();
        _characterController = stateMachine.GetComponent<CharacterController>();
        _interactionManager = stateMachine.GetComponent<InteractionManager>();
        _interactionObject = _interactionManager.currentInteractiveObject.GetComponent<InteractiveObject>();
    }

    public override void Awake(InteractiveObject interactiveObject, GameObject owner)
    {
        _player = owner.GetComponent<Player>();
        _characterController = owner.GetComponent<CharacterController>();
        _interactionManager = owner.GetComponent<InteractionManager>();
        _interactionObject = interactiveObject;
    }

    public override void OnStateEnter()
    {
        // 1. Heavy Object의 Rigidbody 세팅
        _interactiveObjectRigidbody = _interactionObject.GetComponent<Rigidbody>();
        _interactiveObjectRigidbody.constraints = RigidbodyConstraints.FreezeRotation; // Freeze rotation to prevent the box from rolling
        _interactiveObjectRigidbody.useGravity = false; // Prevent box shaking when object is carried

        // 2. Heavy Object의 BoxCollider, Scale 정보 불러오기
        BoxCollider boxCollider = _interactionObject.GetComponent<BoxCollider>();
        Vector3 boxColliderSize = boxCollider.size;
        float boxScale = _interactionObject.transform.localScale.x; // x, y, z는 동일
        Vector3 halfBoxSize = boxColliderSize * (boxScale * 0.5f);  // 절반 크기

        // 3. 플레이어와 Heavy Object 간의 높이 비교. XZ 상으로만 상호작용(World 좌표로 연산)
        Vector3 topWorldPos = _interactionObject.transform.TransformPoint(new Vector3(0f, halfBoxSize.y, 0f));
        Vector3 bottomWorldPos = _interactionObject.transform.TransformPoint(new Vector3(0f, -halfBoxSize.y, 0f));
        float boxTopY    = topWorldPos.y;
        float boxBottomY = bottomWorldPos.y;
        
        float playerY = _player.transform.position.y;
        if (playerY >= boxTopY || playerY <= boxBottomY)
        {
            _isValidState = false;
            _interactionManager.InitCurrentInteraction();
            return;
        }
        
        // 플레이어와 Heavy Object의 거리를 유지할 Offset 계산
        Vector3 interactiveObjectPosition = _interactionObject.transform.position;
        Vector3 playerPosition = _player.transform.position;
        
        // 플레이어 시작 - 박스 끝 벡터
        Vector3 distanceVector = interactiveObjectPosition - playerPosition;

        // Player Collider Half Length
        float playerHalfSize = _characterController.radius;

        // 각 축에 따른 거리 계산
        float distanceX = halfBoxSize.x + playerHalfSize;
        float distanceZ = halfBoxSize.z + playerHalfSize;

        // 로컬 축 벡터
        Vector3 localRight = boxCollider.transform.right;
        Vector3 localForward = boxCollider.transform.forward;

#region Calculate Offset
        // 각 축에 투영된 거리를 계산
        float projectedDistanceX = Vector3.Dot(distanceVector, localRight);
        float projectedDistanceZ = Vector3.Dot(distanceVector, localForward);

        if (Mathf.Abs(projectedDistanceX) > Mathf.Abs(projectedDistanceZ))
        {
            if (projectedDistanceX <= -halfBoxSize.x) // 오른쪽에서 잡음 <-
            {
                _offset = -localRight * distanceX;
            }
            else if (projectedDistanceX >= halfBoxSize.x) // 왼쪽에서 잡음 ->
            {
                _offset = localRight * distanceX;
            }
            else
            {
                Debug.LogWarning("Player and Heavy Collider is overlapped");
                return;
            }
        }
        else
        {
            if (projectedDistanceZ >= halfBoxSize.z) // 앞쪽에서 잡음 ^
            {
                _offset = localForward * distanceZ;
            }
            else if (projectedDistanceZ <= -halfBoxSize.z) // 뒤쪽에서 잡음 v
            {
                _offset = -localForward * distanceZ;
            }
            else
            {
                Debug.LogWarning("Player and Heavy Collider is overlapped");
                return;
            }
        }
#endregion

        // Adjustment Offset for calculating targetPosition
        _offset.y = halfBoxSize.y - _player.transform.localScale.x * _characterController.radius;
        _player.transform.position = interactiveObjectPosition - _offset;
        
        
        // layer를 잠깐 Default로 변경(Wall layer를 탐지하기 때문에 스스로 못 움직임)
        _interactionObject.gameObject.layer = _defaultLayer;
    }

    public override void OnUpdate()
    {
        if (!_isValidState)
            return;
        
        // 오프셋을 이용하여 상호작용 오브젝트의 목표 위치 계산
        Vector3 objectTargetPosition = _player.transform.position + _offset;
        Vector3 direction = (objectTargetPosition - _interactiveObjectRigidbody.position).normalized;
        _newPosition = _interactiveObjectRigidbody.position + direction * _originSO.pullStep;

        // 충돌 검사 및 이동

        // 땅에서의 이동
        if (!Physics.CheckBox(_newPosition, _interactiveObjectRigidbody.transform.localScale / 2, Quaternion.identity, _originSO.collisionLayerMask))
        {
            _interactiveObjectRigidbody.MovePosition(_newPosition);
        }
        // 벽과 접촉한 경우, 벽면에 수직으로 이동
        else
        {
            // 외적으로 진행방향과 수직인 벡터를 구함
            Vector3 slideDirection = Vector3.Cross(Vector3.up, direction).normalized;
            // 내적으로 슬라이드 방향이 음수인지 양수인지 구함
            if (Vector3.Dot(_player.movementInput, slideDirection) < 0)
            {
                slideDirection = -slideDirection;
            }
            else if (Vector3.Dot(_player.movementInput, slideDirection) == 0)
            {
                slideDirection = Vector3.zero;
            }
            
            _newPosition = _interactiveObjectRigidbody.position + slideDirection * _originSO.pullStep;
            
            // 만약 벽이 막고 있지 않다면 이동
            if (!Physics.CheckBox(_newPosition, _interactiveObjectRigidbody.transform.localScale / 2, Quaternion.identity, _originSO.collisionLayerMask))
            {
                _interactiveObjectRigidbody.MovePosition(_newPosition);
            }
        }
        
        // 현재 플레이어-박스 간 거리
        float currentDistance = Vector3.Distance(_player.transform.position, _interactiveObjectRigidbody.position);
        // 최대 허용 거리 = initialDistance + maxPullDistanceMargin
        float maxAllowed = _offset.magnitude + _originSO.maxPullDistanceOffset;
        
        // Player의 위치를 알맞게 조절
        if (_interactiveObjectRigidbody.position == _newPosition || currentDistance > maxAllowed)
        {
            //_player.transform.position = _newPosition - _offset;
            Vector3 targetPlayerPos = _newPosition - _offset;
            _player.transform.position = Vector3.Lerp(
                _player.transform.position,
                targetPlayerPos,
                _originSO.lerpDuration * Time.deltaTime
            );
        }
    }

    public override void OnStateExit()
    {
        // 움직일 때 진동 방지한 것 다시 원래대로 변경
        _interactiveObjectRigidbody.constraints = RigidbodyConstraints.None;
        _interactiveObjectRigidbody.useGravity = true;
        
        // 다시 원래 Layer로 변경
        _interactionObject.gameObject.layer = _wallLayer;
    }
}
