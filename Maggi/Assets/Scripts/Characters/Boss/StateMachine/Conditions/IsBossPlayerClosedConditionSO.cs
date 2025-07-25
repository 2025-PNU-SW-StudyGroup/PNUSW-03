using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;
using Maggi.Character.Boss;

[CreateAssetMenu(fileName = "IsBossPlayerClosedCondition", menuName = "State Machines/Conditions/Boss/Is Boss Player Closed Condition")]
public class IsBossPlayerClosedConditionSO : StateConditionSO
{
    public float distance = 3.0f;

	protected override Condition CreateCondition() => new IsBossPlayerClosedCondition();
}

public class IsBossPlayerClosedCondition : Condition
{
	protected new IsBossPlayerClosedConditionSO _originSO => (IsBossPlayerClosedConditionSO)base.OriginSO;
    private Boss _boss = default;

	public override void Awake(StateMachine stateMachine)
	{
        _boss = stateMachine.GetComponent<Boss>();
	}
	
	protected override bool Statement()
	{
        Vector3 distanceVector = _boss.Target.position - _boss.transform.position;
        distanceVector.y = 0.0f;
        float dist = distanceVector.magnitude;
        if (dist < _originSO.distance)
        {
            _boss.SetMode(Mode.Catch, "is boss player closed condition");
            return true;
        }
        return false;
    }
}
