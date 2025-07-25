using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;
using UnityEngine.AI;
using Maggi.Character.Boss;

[CreateAssetMenu(fileName = "NextAreaMoveAction", menuName = "State Machines/Actions/Boss/Next Area Move Action")]
public class NextAreaMoveActionSO : StateActionSO
{
    public float moveSpeed = 2.0f;
    public float rotationSpeed = 10.0f;
	protected override StateAction CreateAction() => new NextAreaMoveAction();
}

public class NextAreaMoveAction : StateAction
{
	protected new NextAreaMoveActionSO _originSO => (NextAreaMoveActionSO)base.OriginSO;
	
	private Boss _boss;
	private NavMeshAgent _agent;
    private Transform _transform;
	private Transform[] _patrolAreas;
    private int areaIndex = 0;
    
    private const float ROTATE_TRESHOLD = 0.01f;

    public override void Awake(StateMachine stateMachine)
	{
		_boss = stateMachine.GetComponent<Boss>();
		_agent = stateMachine.GetComponent<NavMeshAgent>();
        _transform = stateMachine.GetComponent<Transform>();
	}

    public override void OnStateEnter()
    {
		_patrolAreas = _boss.PatrolAreas[_boss.CurrentRootIndex]; // 이동 가능한 포인트들
        _agent.speed = _originSO.moveSpeed;
    }

    public override void OnUpdate()
	{
		// 다음 목적지로 이동
		_agent.SetDestination(_patrolAreas[areaIndex].position);
        

        // 멈췄다면 Idle 상태로 바꾸고 다음 목적지 갱신
        if (_boss.IsStopped())
        {
            areaIndex = (areaIndex + 1) % _patrolAreas.Length;
            _boss.SetMode(Mode.Idle, "next area move action");
        }
    }
}
