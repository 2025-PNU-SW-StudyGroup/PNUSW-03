using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "IsMaggiRunMovingCondition",menuName = "State Machines/Conditions/MaggiRun/IsMovingInput")]
public class IsMoving_MaggiRunConditionSO : StateConditionSO<IsMoving_MaggiRunCondition> { }

public class IsMoving_MaggiRunCondition : Condition
{
    private PlayerMaggiRun _player;

    public override void Awake(StateMachine stateMachine)
    {
        _player = stateMachine.GetComponent<PlayerMaggiRun>();
    }

    protected override bool Statement()
    {
        return _player != null && _player.InputVector.sqrMagnitude > 0.05f;
    }
}