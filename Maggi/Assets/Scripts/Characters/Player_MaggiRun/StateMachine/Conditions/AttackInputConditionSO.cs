using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machines/Conditions/MaggiRun/Attack Input")]
public class AttackInputConditionSO : StateConditionSO<AttackInputCondition> { }

public class AttackInputCondition : Condition
{
    private InputReaderMaggiRun _input;
    private PlayerMaggiRun _player;
    private bool _attackTriggered = false;

    public override void Awake(StateMachine stateMachine)
    {
        _player = stateMachine.GetComponent<PlayerMaggiRun>();
        _input = _player.InputReader;

        if (_input != null)
        {
            _input.AttackEvent += OnAttack;
        }
    }

    public override void OnStateExit()
    {
        _attackTriggered = false;
    }

    protected override bool Statement()
    {
        return _attackTriggered;
    }

    private void OnAttack()
    {
        _attackTriggered = true;
    }
}