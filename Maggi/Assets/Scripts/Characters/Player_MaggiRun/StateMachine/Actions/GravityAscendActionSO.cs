using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "GravityAscendAction", menuName = "State Machines/Actions/MaggiRun/Gravity Ascend")]
public class GravityAscendActionSO : StateActionSO<GravityAscendAction>
{
    public float initialJumpForce = 15f;
    public float gravityMultiplier = 2f;
    public float airResistance = 0.98f;
}

public class GravityAscendAction : StateAction
{
    private PlayerMaggiRun _player;
    private Rigidbody _rb;
    private GravityAscendActionSO _origin => (GravityAscendActionSO)OriginSO;

    private float _verticalVelocity;
    private float _gravityTimer;

    public override void Awake(StateMachine stateMachine)
    {
        _player = stateMachine.GetComponent<PlayerMaggiRun>();
        _rb = _player.GetComponent<Rigidbody>();
    }

    public override void OnStateEnter()
    {
        _verticalVelocity = _origin.initialJumpForce;
        _gravityTimer = 0f;
        _player.jumpInput = false;

        Vector3 jumpDir = -_player.GravityDirection.normalized;
        Vector3 horizontalVel = Vector3.ProjectOnPlane(_rb.linearVelocity, jumpDir);
        _rb.linearVelocity = horizontalVel + jumpDir * _verticalVelocity;
    }

    public override void OnUpdate()
    {
        Vector3 gravityDir = -_player.GravityDirection.normalized;
        _gravityTimer += Time.deltaTime;

        float gravityEffect = Physics.gravity.magnitude * _origin.gravityMultiplier * _gravityTimer;
        _verticalVelocity -= gravityEffect * Time.deltaTime;

        _verticalVelocity *= _origin.airResistance;

        Vector3 horizontalVel = Vector3.ProjectOnPlane(_rb.linearVelocity, gravityDir);
        _rb.linearVelocity = horizontalVel + gravityDir * _verticalVelocity;
    }
}