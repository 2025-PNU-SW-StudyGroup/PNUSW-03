using UnityEngine;
using UnityEngine.AI;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;
using Maggi.Character.Boss;

[CreateAssetMenu(fileName = "DetectTargetAction", menuName = "State Machines/Actions/Boss/Detect Target Action")]
public class DetectTargetActionSO : StateActionSO<DetectTargetAction> 
{
    public float moveSpeed = 4.0f;
    // 필요하다면 SO 에 파라미터로 뺄 수 있습니다.
    public float sampleRadius = 5f;
}

public class DetectTargetAction : StateAction
{
    protected new DetectTargetActionSO _originSO => (DetectTargetActionSO)base.OriginSO;
    
    private Boss _boss;
    private NavMeshAgent _agent;
    private DetectTargetActionSO _so;

    public override void Awake(StateMachine stateMachine)
    {
        _boss  = stateMachine.GetComponent<Boss>();
        _agent = stateMachine.GetComponent<NavMeshAgent>();
        _so    = (DetectTargetActionSO)OriginSO;
    }

    public override void OnStateEnter()
    {
        _agent.speed = _originSO.moveSpeed;
    }

    public override void OnUpdate()
    {
        if (_boss.Target == null)
            return;

        Vector3 desiredPos = _boss.Target.position;

        // 1) NavMesh 위 가장 가까운 점 찾기
        NavMeshHit hit;
        if (NavMesh.SamplePosition(desiredPos, out hit, _so.sampleRadius, NavMesh.AllAreas))
        {
            // NavMesh 위의 샘플링된 지점으로 이동
            _agent.SetDestination(hit.position);
        }
        else
        {
            // 샘플링 실패 시: 경로 리셋 후 직접 이동
            _agent.ResetPath();
            Vector3 dir = (desiredPos - _agent.transform.position).normalized;
            _agent.transform.position += dir * (_agent.speed * Time.deltaTime);
        }

        // 2) 도착 여부 체크
        if (_boss.IsStopped())
        {
            // Trigger Mode 로 전환
            _boss.SetMode(Mode.Catch, "detect target action");
        }
    }
}