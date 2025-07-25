using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "MountPlayerAction", menuName = "State Machines/Actions/Mount Player Action")]
public class MountPlayerActionSO : StateActionSO<MountPlayerAction> { }

public class MountPlayerAction : StateAction
{
	private Transform _playerTransform;
	private Transform _interactiveObjectTransform;
	private Collider _interactiveObjectCollider;
	private Vector3 _mountPosition;
	public override void Awake(InteractiveObject interactiveObject, GameObject owner)
	{
		_playerTransform = owner.transform;
		_interactiveObjectTransform = interactiveObject.transform;
		_interactiveObjectCollider = _interactiveObjectTransform.GetComponent<Collider>();
	}
	public override void OnUpdate()
	{
	}

	public override void OnStateEnter()
	{
		_playerTransform.SetParent(_interactiveObjectTransform);
		_mountPosition = _interactiveObjectCollider.ClosestPoint(_playerTransform.position);
		_playerTransform.position = _mountPosition;
	}
}
