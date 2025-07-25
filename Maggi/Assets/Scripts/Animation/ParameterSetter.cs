using UnityEngine;
using Moment = Maggi.StateMachine.StateAction.SpecificMoment;
public class ParameterSetter : StateMachineBehaviour
{
    [SerializeField] private ParameterType _parameterType;
    [SerializeField] private string _parameterName;

    [SerializeField] private float _floatValue;
    [SerializeField] private int _intValue;
    [SerializeField] private bool _boolValue;
    [SerializeField] private Moment whenToRun;
    public enum ParameterType { Float, Int, Bool, Trigger }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (whenToRun == Moment.OnStateEnter)
            SetParameter(animator);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (whenToRun == Moment.OnStateExit)
            SetParameter(animator);
    }

    private void SetParameter(Animator animator)
    {
        switch (_parameterType)
        {
            case ParameterType.Bool:
                animator.SetBool(_parameterName, _boolValue);
                break;
            case ParameterType.Int:
                animator.SetInteger(_parameterName, _intValue);
                break;
            case ParameterType.Float:
                animator.SetFloat(_parameterName, _floatValue);
                break;
            case ParameterType.Trigger:
                animator.SetTrigger(_parameterName);
                break;
        }
    }
}
