using UnityEngine;

public class ParameterSetterOnEvent : MonoBehaviour
{
    [SerializeField] private ParameterType _parameterType;
    [SerializeField] private string _parameterName;
    [SerializeField] private float _floatValue;
    [SerializeField] private int _intValue;
    [SerializeField] private bool _boolValue;
    
    [Header("Listening to")]
    [SerializeField] private VoidEventChannelSO _eventChannel;
    
    private Animator _animator;

    public enum ParameterType { Float, Int, Bool, Trigger }

    private void Awake()
    {
        _animator ??= GetComponent<Animator>();
    }
    private void OnEnable()
    {
        _eventChannel.OnEventRaised += SetParameter;
    }
    private void OnDisable()
    {
        _eventChannel.OnEventRaised -= SetParameter;
    }

    private void SetParameter()
    {
        switch (_parameterType)
        {
        case ParameterType.Float:
            _animator.SetFloat(_parameterName, _floatValue);
            break;
        case ParameterType.Int:
            _animator.SetInteger(_parameterName, _intValue);
            break;
        case ParameterType.Bool:
            _animator.SetBool(_parameterName, _boolValue);
            break;
        case ParameterType.Trigger:
            _animator.SetTrigger(_parameterName);
            break;
        }
    }
}