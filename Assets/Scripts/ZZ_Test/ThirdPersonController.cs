using Rubworld.Gravity.Behaviors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    private ThirdPersonActionAsset _playerActionsAsset;
    private InputAction _move;

    private Rigidbody _rb;
    [SerializeField] private float _movementForce = 1f;
    
    private Vector3 _forceDirection = Vector3.zero;

    [SerializeField] private MovingRef _movingRef;
    [SerializeField] private GravityApplier _gravityApplier;
    [SerializeField] private float _noControlDelay = 0.07f;
    [SerializeField] private float _speedMultiplierWhileCooldown = 0.5f;
    [SerializeField] private float _speedMultiplierWhileFalling = 0.1f;
    private bool _isFalling;
    private bool _shouldApplyGravity;
    private float _currentCooldown;
    private float _speedMultiplier;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerActionsAsset = new ThirdPersonActionAsset();
        _gravityApplier = GetComponent<GravityApplier>();
    }

    private void OnEnable()
    {
        Debug.Log("ALO?");
        _move = _playerActionsAsset.Player.Move;
        _playerActionsAsset.Player.Enable();
    }

    private void OnDisable()
    {
        _playerActionsAsset.Player.Disable();
    }

    private void FixedUpdate()
    {
        UpdateIsFalling();
        if (_isFalling) // depending of gravity
        {
            NoMovement();
            return;
        }

        Movement();

        LookForward();
    }

    private void NoMovement()
    {
        // here speed multiplier should be very low
        _forceDirection += GetCameraRight() * (_move.ReadValue<Vector2>().x * _movementForce * Time.deltaTime * 100f * _speedMultiplier);
        _forceDirection += GetCameraForward() * (_move.ReadValue<Vector2>().y * _movementForce * Time.deltaTime * 100f * _speedMultiplier);
        
        _rb.velocity = _forceDirection + _gravityApplier.GetGravityVelocityValue(_rb.velocity);
        _forceDirection = Vector3.zero;
    }

    private void Movement()
    {
        _forceDirection += GetCameraRight() * (_move.ReadValue<Vector2>().x * _movementForce * Time.deltaTime * 100f * _speedMultiplier);
        _forceDirection += GetCameraForward() * (_move.ReadValue<Vector2>().y * _movementForce * Time.deltaTime * 100f * _speedMultiplier);

        _rb.velocity =  _gravityApplier.ApplyGravityVelocity(_forceDirection, .8f);
        _rb.velocity = _forceDirection;
        _forceDirection = Vector3.zero;
    }

    private void LookForward()
    {
        Vector3 direction = _rb.velocity;
        direction = _gravityApplier.RemoveGravityVelocity(_rb.velocity); // depending of gravity

        if (_move.ReadValue<Vector2>().sqrMagnitude == 0f)
        {
            return;
        }

        if (direction.sqrMagnitude > 0.00001f)
        {
            _rb.rotation = Quaternion.LookRotation(direction, transform.up);
        }
        else
        {
            _rb.angularVelocity = Vector3.zero;
        }
    }

    private void UpdateIsFalling()
    {
        if (_gravityApplier.ShouldApplyGravity == false)
        {
            _isFalling = false;
            _shouldApplyGravity = _gravityApplier.ShouldApplyGravity;
            _currentCooldown = _noControlDelay;
            _speedMultiplier = 1f;
            return;
        }
        if (_shouldApplyGravity && _gravityApplier.ShouldApplyGravity)
        {
            _currentCooldown -= Time.deltaTime;
            if (!(_currentCooldown <= 0f))
            {
                _speedMultiplier = _speedMultiplierWhileCooldown;
                _isFalling = false;
            }
            else
            {
                _speedMultiplier = _speedMultiplierWhileFalling;
                _isFalling = true;
                _currentCooldown = 0f;
            }
        }

        _shouldApplyGravity = _gravityApplier.ShouldApplyGravity;
    }

    private Vector3 GetCameraRight()
    {
        Vector3 right = _movingRef.VectorRight;
        Debug.DrawRay(transform.position + transform.up * 0.05f, right * 2f, Color.blue);
        return right.normalized;
    }

            
    private Vector3 GetCameraForward()
    {
        Vector3 forward = _movingRef.VectorUp;
        Debug.DrawRay(transform.position + transform.up * 0.05f, forward * 2f, Color.cyan);
        return forward.normalized;
    }

    private void DoJump(InputAction.CallbackContext callbackContext)
    {
        if (IsGrounded() == false)
        {
            return;
        }
        
        // todo
        Debug.Log($"[FIX] Jump.callbackContext\n" +
                  $"{{\n" +
                  $"  action : {callbackContext.action}\n" +
                  $"  canceled : {callbackContext.canceled}\n" +
                  $"  control : {callbackContext.control}\n" +
                  $"  duration : {callbackContext.duration}\n" +
                  $"  interaction : {callbackContext.interaction}\n" +
                  $"  performed : {callbackContext.performed}\n" +
                  $"  phase : {callbackContext.phase}\n" +
                  $"  started : {callbackContext.started}\n" +
                  $"  time : {callbackContext.time}\n" +
                  $"  startTime : {callbackContext.startTime}\n" +
                  $"  valueType : {callbackContext.valueType}\n" +
                  $"  valueSizeInBytes : {callbackContext.valueSizeInBytes}\n" +
                  $"}}");
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.25f, Vector3.down); // depending of gravity
        return Physics.Raycast(ray, out RaycastHit hit, 0.3f);
    }
}
