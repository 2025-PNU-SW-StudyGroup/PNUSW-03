using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "PullLightAction", menuName = "State Machines/Actions/PullLightAction")]
public class PullLightActionSO : StateActionSO<PullLightAction> 
{
    public LayerMask mouseClickLayerMask;
}

public class PullLightAction : StateAction
{
	protected new PullLightActionSO _originSO => (PullLightActionSO)base.OriginSO;
	private Player _player;
	private InteractionManager _interactionManager;
    private InteractiveObject _interactiveObject;
    private Collider _interactiveObjectCollider;
    private Rigidbody _interactiveObjectRigidbody;
    private Camera _mainCamera;

	public override void Awake(StateMachine stateMachine)
	{
		_player = stateMachine.GetComponent<Player>();
        _interactionManager = stateMachine.GetComponent<InteractionManager>();
    }

    public override void Awake(InteractiveObject interactiveObject, GameObject owner)
    {
        _player = owner.GetComponent<Player>();
        _interactionManager = owner.GetComponent<InteractionManager>();
        _interactiveObject = interactiveObject;
    }

    public override void OnStateEnter()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;
        
        _interactiveObjectCollider = _interactiveObject.GetComponent<Collider>();
        _interactiveObjectCollider.enabled = false;

        _interactiveObjectRigidbody = _interactiveObject.GetComponent<Rigidbody>();

        // When Pulling Light Object, it needs to remove from list of Potential Interactions
        _interactionManager.OnTriggerChangeDetected(false, _interactionManager.currentInteractiveObject);
    }

    public override void OnUpdate()
    {
        // 1) 플레이어 높이 평면 (아래쪽으로만 교차)
        Plane lowPlane  = new Plane(Vector3.up,    _player.transform.position);
        // 2) 플레이어 머리 위 적당한 높이에 평면 추가 (위쪽으로만 교차)
        float headY     = _player.transform.position.y + 10f; 
        Plane highPlane = new Plane(Vector3.down,  new Vector3(0, headY, 0));

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 mouseWorldPosition;
        float   distance;

        // 3) 아래→위 순서로 시도
        if (lowPlane.Raycast(ray, out distance) && distance > 0f)
        {
            mouseWorldPosition = ray.GetPoint(distance);
        }
        else if (highPlane.Raycast(ray, out distance) && distance > 0f)
        {
            mouseWorldPosition = ray.GetPoint(distance);
        }
        else
        {
            // 그래도 못 잡으면 적당한 거리(예: 5m)로 뽑아두기
            mouseWorldPosition = ray.GetPoint(5f);
        }

        // 4) 결과 반영
        Vector3 dir = (mouseWorldPosition - _player.transform.position).normalized;
        _interactiveObject.transform.position = _player.transform.position
            + dir * 0.4f
            + Vector3.up * 0.2f;
        _interactiveObject.transform.LookAt(mouseWorldPosition);
    }


    public override void OnStateExit()
    {
        if (_interactiveObjectCollider != null)
            _interactiveObjectCollider.enabled = true;
        if (_interactiveObjectRigidbody != null)
        {
            _interactiveObjectRigidbody.isKinematic = false;
        }
    }
}
