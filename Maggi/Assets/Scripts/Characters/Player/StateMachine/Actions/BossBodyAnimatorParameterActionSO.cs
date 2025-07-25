using Maggi.Character.Boss;
using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;
using Moment = Maggi.StateMachine.StateAction.SpecificMoment;

[CreateAssetMenu(fileName = "BossBodyAnimatorParameterAction", menuName = "State Machines/Actions/Boss/Boss Body Animator Parameter Action")]
public class BossBodyAnimatorParameterActionSO : StateActionSO
{
	public ParameterType parameterType;
	public string parameterName;

    public bool boolValue;
    public int intValue;
    public float floatValue;

	public Moment whenToRun;

    protected override StateAction CreateAction() => new BossBodyAnimatorParameterAction(Animator.StringToHash(parameterName));

	public enum ParameterType
	{
		Bool, Int, Float, Trigger
	};
}

public class BossBodyAnimatorParameterAction : StateAction
{
	protected new BossBodyAnimatorParameterActionSO _originSO => (BossBodyAnimatorParameterActionSO)base.OriginSO;
	private Animator _animator;
	private int _parameterHash;

	public BossBodyAnimatorParameterAction(int parameterHash)
	{
		_parameterHash = parameterHash;
	}

	public override void Awake(StateMachine stateMachine)
	{
		_animator = stateMachine.GetComponent<Boss>().HandAnimator;
	}

    public override void OnUpdate() { }
	
	public override void OnStateEnter()
	{
		if (_originSO.whenToRun == SpecificMoment.OnStateEnter)
		{
			SetParameter();
		}
	}
	
	public override void OnStateExit()
	{
        if (_originSO.whenToRun == SpecificMoment.OnStateExit)
            SetParameter();
    }

	private void SetParameter()
	{
		switch (_originSO.parameterType)
		{
        case BossBodyAnimatorParameterActionSO.ParameterType.Bool:
            _animator.SetBool(_parameterHash, _originSO.boolValue);
            break;
        case BossBodyAnimatorParameterActionSO.ParameterType.Int:
            _animator.SetInteger(_parameterHash, _originSO.intValue);
            break;
        case BossBodyAnimatorParameterActionSO.ParameterType.Float:
            _animator.SetFloat(_parameterHash, _originSO.floatValue);
            break;
        case BossBodyAnimatorParameterActionSO.ParameterType.Trigger:
            _animator.SetTrigger(_parameterHash);
            break;
        }
	}
}
