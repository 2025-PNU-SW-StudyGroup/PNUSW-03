using UnityEngine;
using UnityEngine.Events;

public class RaiseEventOnStateExit : StateMachineBehaviour
{
    [SerializeField] private UnityEvent _eventChannel;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _eventChannel?.Invoke();
    }
}
