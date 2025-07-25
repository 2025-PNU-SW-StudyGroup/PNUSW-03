using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "NotGroundedCondition", menuName = "State Machines/Conditions/MaggiRun/Maggi Not Grounded")]
public class NotGroundedConditionSO : StateConditionSO<NotGroundedCondition> { }

public class NotGroundedCondition : Condition
{
    private PlayerMaggiRun _player;

    public override void Awake(StateMachine stateMachine)
    {
        _player = stateMachine.GetComponent<PlayerMaggiRun>();
    }

    protected override bool Statement()
    {
        return !_player.isGrounded;
    }
}