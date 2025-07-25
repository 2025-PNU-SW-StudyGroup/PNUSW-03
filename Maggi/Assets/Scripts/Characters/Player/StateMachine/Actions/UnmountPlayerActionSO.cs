using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "UnmountPlayerAction", menuName = "State Machines/Actions/Unmount Player Action")]
public class UnmountPlayerActionSO : StateActionSO
{
	protected override StateAction CreateAction() => new UnmountPlayerAction();
}

public class UnmountPlayerAction : StateAction
{
	protected new UnmountPlayerActionSO _originSO => (UnmountPlayerActionSO)base.OriginSO;
	private Transform _playerTransform;
	private InteractionManager _interactionManager;
	private Transform _interactiveObjectTransform;
	public override void Awake(InteractiveObject interactiveObject, GameObject owner)
	{
		_playerTransform = owner.transform;
		_interactionManager = owner.GetComponent<InteractionManager>();
		_interactiveObjectTransform = interactiveObject.transform;
	}
	public override void OnUpdate() { }
	public override void OnStateExit()
	{
		// SetParent(null) will not trigger the OnTriggerExit event, 
		// so we need to handle it manually
		_playerTransform.SetParent(null);
		_interactionManager.OnTriggerChangeDetected(false, _interactiveObjectTransform.gameObject);
		_playerTransform.rotation = Quaternion.identity;
	}
}
