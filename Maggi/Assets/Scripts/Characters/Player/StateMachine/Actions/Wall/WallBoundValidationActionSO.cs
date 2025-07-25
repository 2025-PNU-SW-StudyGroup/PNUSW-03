using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "WallBoundValidationAction", menuName = "State Machines/Actions/Wall Bound Validation Action")]
public class WallBoundValidationActionSO : StateActionSO
{
    public LayerMask wallLayerMask;

	protected override StateAction CreateAction() => new WallBoundValidationAction();
}

public class WallBoundValidationAction : StateAction
{
	private new WallBoundValidationActionSO _originSO => (WallBoundValidationActionSO)base.OriginSO;

    private Player _player;
    private InteractionManager _interactionManager;

    public override void Awake(InteractiveObject interactiveObject, GameObject owner)
    {
        _player = owner.GetComponent<Player>();
        _interactionManager = owner.GetComponent<InteractionManager>();
    }

    public override void OnUpdate()
    {
        // 레이 시작점과 방향 정의
        Vector3 origin    = _player.transform.position - _player.transform.right * 0.2f;
        Vector3 direction = -_player.transform.up;
        float   maxDist   = 2.0f;

        // ——— 디버깅용 시각화 —————————————————————————
        Debug.DrawRay(origin, direction * maxDist, Color.red);
        // 원하는 경우 충돌 지점도 표시
        if (Physics.Raycast(origin, direction, out RaycastHit debugHit, maxDist, _originSO.wallLayerMask))
        {
            Debug.DrawLine(origin, debugHit.point, Color.green);
        }
        // ————————————————————————————————————————————————

        // 만약 벽 위에 있으면 상태 유지
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDist, _originSO.wallLayerMask))
            return;

        Debug.Log("벗어남 뱀~");

        // 벽 범위를 벗어나면 상호작용 종료
        _interactionManager.InitCurrentInteraction();
    }

}
