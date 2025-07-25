using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "GravityDescendAction", menuName = "State Machines/Actions/MaggiRun/Gravity Descend")]
public class GravityDescendActionSO : StateActionSO<GravityDescendAction>
{
    public float gravityMultiplier = 2f;
    public float airResistance = 0.98f;
    public float maxFallSpeed = -50f;
}

public class GravityDescendAction : StateAction
{
    private PlayerMaggiRun _player;
    private Rigidbody _rb;
    private GravityDescendActionSO _origin => (GravityDescendActionSO)OriginSO;

    private float _verticalVelocity;

    public override void Awake(StateMachine stateMachine)
    {
        _player = stateMachine.GetComponent<PlayerMaggiRun>();
        _rb = _player.GetComponent<Rigidbody>();
    }

    public override void OnStateEnter()
    {
        _verticalVelocity = Vector3.Dot(_rb.linearVelocity, Physics.gravity.normalized);
        _player.jumpInput = false;
    }

    public override void OnUpdate()
    {
        Vector3 gravityDir = Physics.gravity.normalized;
        float gravityForce = Physics.gravity.magnitude * _origin.gravityMultiplier;

        _verticalVelocity += gravityForce * Time.deltaTime;
        _verticalVelocity *= _origin.airResistance;
        _verticalVelocity = Mathf.Clamp(_verticalVelocity, _origin.maxFallSpeed, float.MaxValue);

        Vector3 horizontalVel = Vector3.ProjectOnPlane(_rb.linearVelocity, gravityDir);
        _rb.linearVelocity = horizontalVel + gravityDir * _verticalVelocity;
    }
}
