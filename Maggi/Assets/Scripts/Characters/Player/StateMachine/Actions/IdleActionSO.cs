using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "IdleAction", menuName = "State Machines/Actions/IdleAction")]
public class IdleActionSO : StateActionSO
{
	protected override StateAction CreateAction() => new IdleAction();
}

public class IdleAction : StateAction
{

	protected new IdleActionSO OriginSO => (IdleActionSO)base.OriginSO;
    private Player _playerScript;
    private InteractionManager _interactionManager;
    public override void Awake(StateMachine stateMachine)
    {
        _playerScript = stateMachine.GetComponent<Player>();
        _interactionManager = stateMachine.GetComponent<InteractionManager>();
    }

    public override void OnStateEnter()
    {
		_interactionManager.currentInteractionType = InteractionType.None;
		_interactionManager.currentInteractiveObject = null;
        
        _playerScript.movementVector = Vector3.zero;
    }
    public override void OnUpdate() { }
}
