using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "IsGroundCondition", menuName = "State Machines/Conditions/MaggiRun/IsMaggiGrounded")]
public class IsGroundedMaggiRunConditionSO : StateConditionSO<IsGroundedMaggiRunCondition> { }

public class IsGroundedMaggiRunCondition : Condition
{
    private PlayerMaggiRun _player;

    public override void Awake(StateMachine stateMachine)
    {
        _player = stateMachine.GetComponent<PlayerMaggiRun>();
    }

    protected override bool Statement()
    {
        return _player.IsGrounded;
    }
}