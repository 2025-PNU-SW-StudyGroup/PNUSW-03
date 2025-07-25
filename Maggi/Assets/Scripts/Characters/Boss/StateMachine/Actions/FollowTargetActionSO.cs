using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;
using Maggi.Character.Boss;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

[CreateAssetMenu(fileName = "FollowTargetAction", menuName = "State Machines/Actions/Boss/Follow Target Action")]
public class FollowTargetActionSO : StateActionSO<FollowTargetAction> 
{
    public float speed = 4.0f;
}

public class FollowTargetAction : StateAction
{
	protected new FollowTargetActionSO _originSO => (FollowTargetActionSO)base.OriginSO;

    private Boss _boss;
    private Animator _anim;
    private bool _hasPlayedCatch;
    private const float CATCH_DISTANCE = 0.5f;

	public override void Awake(StateMachine stateMachine)
	{
        _boss = stateMachine.GetComponent<Boss>();
        _anim = _boss.HandAnimator;
	}

    public override void OnUpdate()
	{
        Vector3 currentPos = _boss.HandIK_Catch.data.target.transform.position;
        Vector3 targetPos = _boss.Target.position;

        Vector3 newPos = Vector3.Lerp(currentPos, targetPos, _originSO.speed * Time.deltaTime);
        _boss.HandIK_Catch.data.target.transform.position = newPos;
        
        Debug.Log("왜 못 잡음?");
        
        if (!_hasPlayedCatch && (newPos - targetPos).magnitude < CATCH_DISTANCE)
        {
            Debug.Log("Play Boss Mecha Arm Catch");
            _anim.Play("RightMechaArmCatch");
            _hasPlayedCatch = true;
        }
        else
        {
            Debug.Log($"has played catch = {_hasPlayedCatch}");
        }
    }
}
