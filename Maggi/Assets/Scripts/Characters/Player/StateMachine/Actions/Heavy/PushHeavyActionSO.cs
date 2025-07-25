using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "PushHeavyAction", menuName = "State Machines/Actions/Push Heavy Action")]
public class PushHeavyActionSO : StateActionSO<PushHeavyAction> { }

public class PushHeavyAction : StateAction
{
    private InteractionManager _interactionManager;

    public override void Awake(StateMachine stateMachine)
    {
        _interactionManager = stateMachine.GetComponent<InteractionManager>();
    }

    public override void Awake(InteractiveObject interactiveObject, GameObject owner)
    {
        _interactionManager = owner.GetComponent<InteractionManager>();
    }

    public override void OnStateExit()
    {
        _interactionManager.InitCurrentInteraction();
    }

    public override void OnUpdate() { }
}
