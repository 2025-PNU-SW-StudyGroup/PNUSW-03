using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "MaggiRunDieAction", menuName = "State Machines/Actions/MaggiRun/MaggiRun Die")]
public class MaggiDieActionSO : StateActionSO<MaggiDieAction> { }

public class MaggiDieAction : StateAction
{
    private PlayerMaggiRun _player;

    public override void Awake(StateMachine stateMachine)
    {
        _player = stateMachine.GetComponent<PlayerMaggiRun>();
    }

    public override void OnStateEnter()
    {
        _player.Die();
    }
    public override void OnUpdate() { }
    public override void OnFixedUpdate() { }
}