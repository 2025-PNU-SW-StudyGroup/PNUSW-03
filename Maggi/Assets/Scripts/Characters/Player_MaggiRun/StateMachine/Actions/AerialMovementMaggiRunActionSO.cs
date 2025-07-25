using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;

[CreateAssetMenu(fileName = "AerialMovementMaggiRunAction", menuName = "State Machines/Actions/MaggiRun/Aerial Movement MaggiRun Action")]
public class AerialMovementActionMaggiRunSO : StateActionSO<AerialMovementMaggiRunAction>
{
    public float Speed => _speed;
    public float Acceleration => _acceleration;

    [SerializeField][Range(0.1f, 100f)] private float _speed = 10f;
    [SerializeField][Range(0.1f, 100f)] private float _acceleration = 20f;
}

public class AerialMovementMaggiRunAction : StateAction
{
    private PlayerMaggiRun _player;
    private Rigidbody _rb;
    private AerialMovementActionMaggiRunSO _origin => (AerialMovementActionMaggiRunSO)OriginSO;

    public override void Awake(StateMachine stateMachine)
    {
        _player = stateMachine.GetComponent<PlayerMaggiRun>();
        _rb = stateMachine.GetComponent<Rigidbody>();
    }

    public override void OnUpdate()
    {
        if (_player == null || _rb == null || _player.IsGrounded) return;

        Vector3 gravityDir = Physics.gravity.normalized;

        // 안정적 forward 방향 보정
        Vector3 forward = _player.MoveDirection.sqrMagnitude > 0.01f
            ? _player.MoveDirection
            : Vector3.ProjectOnPlane(_player.transform.forward, gravityDir).normalized;

        Vector3 right = Vector3.Cross(-gravityDir, forward).normalized;

        float inputX = _player.InputVector.x;
        if (Mathf.Abs(inputX) > 0.01f)
        {
            Vector3 horizontal = right * inputX * _origin.Speed;
            Vector3 gravityVel = Vector3.Project(_rb.linearVelocity, gravityDir);

            _rb.linearVelocity = horizontal + gravityVel;
        }
    }
}
