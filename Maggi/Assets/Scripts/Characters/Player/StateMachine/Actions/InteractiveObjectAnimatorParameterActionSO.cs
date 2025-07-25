using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;
using Moment = Maggi.StateMachine.StateAction.SpecificMoment;

[CreateAssetMenu(fileName = "InteractiveObjectAnimatorParameterAction", menuName = "State Machines/Actions/Interactive Object Animator Parameter Action")]
public class InteractiveObjectAnimatorParameterActionSO : StateActionSO
{
	public ParameterType parameterType;
	public string parameterName;
    public bool boolValue;
    public int intValue;
    public float floatValue;
	public Moment whenToRun;
	public enum ParameterType
	{
		Bool, Int, Float, Trigger
	};
    protected override StateAction CreateAction() => new InteractiveObjectAnimatorParameterAction(Animator.StringToHash(parameterName));
}

public class InteractiveObjectAnimatorParameterAction : StateAction
{
	protected new InteractiveObjectAnimatorParameterActionSO _originSO => (InteractiveObjectAnimatorParameterActionSO)base.OriginSO;
	private Animator _animator;
	private int _parameterHash;

	public InteractiveObjectAnimatorParameterAction(int parameterHash)
	{
		_parameterHash = parameterHash;
	}

	public override void Awake(InteractiveObject interactiveObject, GameObject owner)
	{
		_animator = interactiveObject.GetComponentInParent<Animator>();
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
		case InteractiveObjectAnimatorParameterActionSO.ParameterType.Bool:
			_animator.SetBool(_parameterHash, _originSO.boolValue);
			break;
		case InteractiveObjectAnimatorParameterActionSO.ParameterType.Int:
			_animator.SetInteger(_parameterHash, _originSO.intValue);
			break;
		case InteractiveObjectAnimatorParameterActionSO.ParameterType.Float:
			_animator.SetFloat(_parameterHash, _originSO.floatValue);
			break;
		case InteractiveObjectAnimatorParameterActionSO.ParameterType.Trigger:
			_animator.SetTrigger(_parameterHash);
			break;
		}
	}
}
