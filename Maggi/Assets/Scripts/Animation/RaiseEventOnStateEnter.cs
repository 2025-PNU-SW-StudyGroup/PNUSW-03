using UnityEngine;
using UnityEngine.Events;

public class RaiseEventOnStateEnter : StateMachineBehaviour
{
    [SerializeField] private UnityEvent _eventChannel;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _eventChannel?.Invoke();
    }
}
