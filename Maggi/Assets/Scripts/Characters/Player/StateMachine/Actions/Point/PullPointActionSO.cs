using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "PullPointAction", menuName = "State Machines/Actions/Pull Point Action")]

public class PullPointActionSO : StateActionSO<PullPointAction> { }

public class PullPointAction : StateAction
{
    private Transform _transform;
	private InteractionManager _interactionManager;

	public override void Awake(InteractiveObject interactiveObject, GameObject owner)
	{
        _interactionManager = owner.GetComponent<InteractionManager>();
        _transform = owner.GetComponent<Transform>();
	}

    public override void OnStateEnter()
    {
        _transform.rotation = _interactionManager.currentInteractiveObject.transform.rotation;
        _transform.position = _interactionManager.currentInteractiveObject.transform.position + _transform.up * 0.4f;
    }

    public override void OnUpdate() { }
}