using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "PullNormalAction", menuName = "State Machines/Actions/Pull Normal Action")]
public class PullNormalActionSO : StateActionSO
{
	protected override StateAction CreateAction() => new PullNormalAction();
}

public class PullNormalAction : StateAction
{
	protected new PullNormalActionSO _originSO => (PullNormalActionSO)base.OriginSO;
    private InteractionEventListener _interactionEventListener;
    
    
    public override void Awake(InteractiveObject interactiveObject, GameObject owner)
    {
        _interactionEventListener = interactiveObject.GetComponent<InteractionEventListener>();
    }
    
	public override void Awake(StateMachine stateMachine)
	{
	}
	
	public override void OnUpdate()
	{
        
    }
	
	public override void OnStateEnter()
	{
        if (_interactionEventListener != null && _interactionEventListener.enabled)
        {
            _interactionEventListener.OnInteract();
        }
        else
        {
            Debug.Log("Interaction Event Listener is null");
        }
	}
	
	public override void OnStateExit()
	{
        
    }
}
