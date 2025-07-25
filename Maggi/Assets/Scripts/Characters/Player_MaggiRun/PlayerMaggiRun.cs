using UnityEngine;
using System;

public class PlayerMaggiRun : MonoBehaviour
{
    [SerializeField] private InputReaderMaggiRun _inputReader;
    public InputReaderMaggiRun InputReader => _inputReader;
    [SerializeField] private VoidEventChannelSO slowWallClearedEvent;
    [SerializeField] private SceneRestartEventChannelSO sceneRestartEvent;
    [SerializeField] private float gravityStrength = 20f;

    [HideInInspector] public bool jumpInput;
    [HideInInspector] public bool isGrounded;

    public bool IsGrounded => isGrounded;
    public bool InSlowMode => _inSlowMode;
    public bool IsDead => _isDead;
    public Vector3 MoveDirection => _moveDirection;
    public bool NeedsGravityRotation => _needsRotationUpdate;
    public Quaternion GetTargetRotation() => _targetRotation;
    public void FinishGravityRotation() => _needsRotationUpdate = false;
    public Vector2 InputVector => _inputVector;

    private Rigidbody _rb;
    private Vector2 _inputVector;
    private Vector3 _gravityDirection = Vector3.down;
    public Vector3 GravityDirection => _gravityDirection;

    private Vector3 _moveDirection = Vector3.forward;
    private Quaternion _targetRotation;
    private bool _needsRotationUpdate;
    private float _gravityLockTimer;

    private bool _inSlowMode = false;
    private bool _isDead = false;
    private GameObject _currentSlowWall = null;

    private bool _blockUpdateGravity = false;
    private float _blockTimer = 0f;
    private const float BLOCK_DURATION = 0.2f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _gravityDirection = Vector3.down;
        Physics.gravity = _gravityDirection * gravityStrength;

        _moveDirection = Vector3.forward;
    }

    private void OnEnable()
    {
        _inputReader.MoveEvent += OnMovement;
        _inputReader.JumpEvent += () => { if (!_inSlowMode) jumpInput = true; };
        _inputReader.JumpCancelEvent += () => jumpInput = false;
        _inputReader.MouseStrafeEvent += OnMouseStrafe;
        _inputReader.AttackEvent += HandleAttack;
    }

    private void OnDisable()
    {
        _inputReader.MoveEvent -= OnMovement;
        _inputReader.JumpEvent -= () => jumpInput = true;
        _inputReader.JumpCancelEvent -= () => jumpInput = false;
        _inputReader.MouseStrafeEvent -= OnMouseStrafe;
        _inputReader.AttackEvent -= HandleAttack;
    }

    private void Update()
    {
        if (_gravityLockTimer > 0f)
            _gravityLockTimer -= Time.deltaTime;

        if (_blockUpdateGravity)
        {
            _blockTimer -= Time.deltaTime;
            if (_blockTimer <= 0f)
                _blockUpdateGravity = false;
        }
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, -_gravityDirection, Color.cyan, 0.1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
            Time.timeScale = 1f;
            sceneRestartEvent.RaiseEvent();
            return;
        }

        if (collision.gameObject.CompareTag("Face"))
        {
            foreach (var contact in collision.contacts)
            {
                Vector3 normal = contact.normal.normalized;
                float alignment = Vector3.Dot(normal, -_gravityDirection);

                if (alignment > 0.01f)
                {
                    UpdateGravity(normal);
                    isGrounded = true;
                    return;
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Face"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Face"))
        {
            isGrounded = false;
        }
    }

    private void UpdateGravity(Vector3 surfaceNormal)
    {
        Vector3 newUp = surfaceNormal.normalized;
        _gravityDirection = -newUp;
        Physics.gravity = _gravityDirection * gravityStrength;

        Quaternion targetRot = Quaternion.LookRotation(transform.forward, newUp);
        _rb.MoveRotation(targetRot);
    }

    private void OnMouseStrafe(int direction)
    {
        if (_inSlowMode) return;
        _inputVector = new Vector2(direction, 0);
    }

    private void OnMovement(Vector2 movement)
    {
        if (_inSlowMode) return;
        _inputVector = movement;
    }

    private void HandleAttack()
    {
        if (_inSlowMode)
        {
            ExitSlowMode();
        }
    }

    public void FlipGravity()
    {
        _blockUpdateGravity = true;
        _blockTimer = BLOCK_DURATION;

        _gravityDirection = -_gravityDirection;
        Physics.gravity = _gravityDirection * gravityStrength;

        Vector3 projected = Vector3.ProjectOnPlane(transform.forward, _gravityDirection).normalized;
        _moveDirection = projected == Vector3.zero ? Vector3.forward : projected;

        _targetRotation = Quaternion.LookRotation(_moveDirection, -_gravityDirection);
        _needsRotationUpdate = true;

        transform.position += -_gravityDirection * 0.3f;

        _rb.linearVelocity = Vector3.zero;
        _rb.AddForce(-_gravityDirection * 10f, ForceMode.VelocityChange);
    }

    public void EnterSlowMode(GameObject slowWallRoot = null)
    {
        if (_inSlowMode) return;

        Time.timeScale = 0.5f;
        _inSlowMode = true;
        _currentSlowWall = slowWallRoot;
    }

    public void ExitSlowMode()
    {
        if (!_inSlowMode) return;

        Time.timeScale = 1f;
        _inSlowMode = false;

        if (_currentSlowWall != null)
            Destroy(_currentSlowWall);

        _currentSlowWall = null;
        slowWallClearedEvent?.RaiseEvent();

        if (_inputReader != null)
        {
            _inputVector = _inputReader.CurrentMoveValue;
        }
    }

    public void Die()
    {
        _isDead = true;
        Time.timeScale = 1f;
    }
}