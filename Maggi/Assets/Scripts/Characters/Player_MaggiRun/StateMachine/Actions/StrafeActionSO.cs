using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "StrafeAction", menuName = "State Machines/Actions/MaggiRun/Strafe Move")]
public class StrafeActionSO : StateActionSO<StrafeAction>
{
    public float sideSpeed = 20f;
}

public class StrafeAction : StateAction
{
    private PlayerMaggiRun _player;
    private Rigidbody _rb;
    private StrafeActionSO _origin => (StrafeActionSO)OriginSO;

    public override void Awake(StateMachine stateMachine)
    {
        _player = stateMachine.GetComponent<PlayerMaggiRun>();
        _rb = stateMachine.GetComponent<Rigidbody>();
    }

    public override void OnUpdate() { }

    public override void OnFixedUpdate()
    {
        if (_player == null || _rb == null) return;

        Vector3 gravityDir = _player.GravityDirection.normalized;

        Vector3 forward = _player.MoveDirection.sqrMagnitude > 0.01f
            ? _player.MoveDirection
            : _player.transform.forward;

        Vector3 right = Vector3.Cross(-gravityDir, forward).normalized;

        float strafeInput = _player.InputVector.x;
        if (!_player.NeedsGravityRotation && Mathf.Abs(strafeInput) > 0.01f)
        {
            Vector3 strafeMove = right * strafeInput * _origin.sideSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(_rb.position + strafeMove);
        }

        if (_player.NeedsGravityRotation)
        {
            Quaternion targetRot = _player.GetTargetRotation();
            _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, targetRot, 360f * Time.fixedDeltaTime));

            if (Quaternion.Angle(_rb.rotation, targetRot) < 0.1f)
                _player.FinishGravityRotation();
        }
    }
}