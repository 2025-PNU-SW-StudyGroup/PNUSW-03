using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "SlowIdleAction", menuName = "State Machines/Actions/MaggiRun/Slow Idle")]
public class SlowIdleActionSO : StateActionSO<SlowIdleAction>
{
    public float moveSpeed = 4f;
    public float lerpSpeed = 4f;
}

public class SlowIdleAction : StateAction
{
    private PlayerMaggiRun _player;
    private Rigidbody _rb;
    private SlowIdleActionSO _origin => (SlowIdleActionSO)OriginSO;

    public override void Awake(StateMachine stateMachine)
    {
        _player = stateMachine.GetComponent<PlayerMaggiRun>();
        _rb = stateMachine.GetComponent<Rigidbody>();
    }

    public override void OnUpdate() { }

    public override void OnFixedUpdate()
    {
        if (_player == null || _rb == null || _player.IsDead) return;

        Vector3 gravityUp = -Physics.gravity.normalized;
        Vector3 right = Vector3.Cross(gravityUp, _player.MoveDirection).normalized;
        Vector3 inputMove = right * _player.InputVector.x * _origin.moveSpeed;
        Vector3 move = Vector3.ProjectOnPlane(inputMove, gravityUp);
        Vector3 gravityVel = Vector3.Project(_rb.linearVelocity, gravityUp);

        Vector3 targetVel = move + gravityVel;
        _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, targetVel, _origin.lerpSpeed * Time.fixedDeltaTime);

        RotateToGravity();
    }

    private void RotateToGravity()
    {
        if (!_player.NeedsGravityRotation) return;

        Quaternion targetRot = _player.GetTargetRotation();
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, targetRot, 360f * Time.fixedDeltaTime));

        if (Quaternion.Angle(_rb.rotation, targetRot) < 0.1f)
            _player.FinishGravityRotation();
    }
}