using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPhysics : MonoBehaviour
{
    private static readonly int AnimIdleId = Animator.StringToHash("Is_idle");
    private static readonly int AnimRunningId = Animator.StringToHash("Is_running");
    private static readonly int AnimJumpingId = Animator.StringToHash("Is_jumping");
    private static readonly int AnimFallingId = Animator.StringToHash("Is_falling");
    
    [Header("Layers References")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField]private LayerMask wallLayer;
    
    [Header("Objects References")]
    [SerializeField] private new CinemachineCamera camera;
    [SerializeField] private Transform leftWallCheck;
    [SerializeField] private Transform rightWallCheck;
    
    [Header("Abilities")] 
    [SerializeField] private bool canRun = true;
    [SerializeField] private bool canWallJump = true;
    
    [Header("Walk Settings")] 
    [SerializeField] private float maxWalkSpeed = 8f;
    [SerializeField] private float walkAccelerationTime = 0.25f;
    [SerializeField] private float walkDecelerationTime = 0.15f;
    
    [Header("Run Settings")] 
    [SerializeField] private float maxRunSpeed = 12f;
    [SerializeField] private float runAccelerationTime = 0.2f;
    [SerializeField] private float runDecelerationTime = 0.2f;
    [SerializeField] private float timeBeforeRunning = 0.3f;
    
    [Header("Jump Settings")] 
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float timeToApex = 0.4f;
    [SerializeField] private float variableHeightRatio = 0.5f;
    
    [Header("Falling Settings")] 
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private float terminalSpeed = -5f;
    [SerializeField] private float coyoteTimeWindow = 0.15f;
    
    [Header("WallJump Settings")] 
    [SerializeField] private float wallJumpMultiplierX = 1f;
    [SerializeField] private float wallJumpMultiplierY = 1.2f;
    
    [Header("Ground Detection")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private float safePosUpdateTime = 0.75f;
    
    [Header("Wall Detection")]
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.1f, 0.5f);
    
    [Header("Camera Settings")]
    [SerializeField] float camOffsetX = 5;
    [SerializeField] float camOffsetY = 5;
    [SerializeField] float camOffsetAccel = 0.1f;
    
    [Header("Debug Values")]
    [SerializeField] public float currentJumpVelocity;
    [SerializeField] public Vector2 currentSpeed;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isOnPlatform;
    [SerializeField] private bool isNearLeftWall;
    [SerializeField] private bool isNearRightWall;
    [SerializeField] private bool isRunning;

    //References
    private CinemachinePositionComposer _positionComposer;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;
    private PlayerDamage _playerDamage;
    
    //Coroutines
    private Coroutine _coyoteCoroutine;
    private Coroutine _safePosCoroutine;
    
    //Inputs
    private float _horizontalInput;
    private bool _jumpInput;
    private bool _rollInput;

    //Movement smoothing
    private float _velXSmooth;
    private float _velX;
    
    //Camera smoothing
    private float _camXSmooth;
    private float _camYSmooth;
    float _targetOffsetX;
    float _targetOffsetY;

    //Derived jump values
    private float _gravity;
    private float _baseGravityScale;
    private float _jumpVelocity;
    private bool _wasGrounded;

    //Position of the ground and wall checks
    private Vector2 GroundCheckPosition => transform.position;
    private Vector2 _rightWallCheckPosition;
    private Vector2 _leftWallCheckPosition;

    void Awake()
    {
        //Get Components
        _rigidbody = GetComponent<Rigidbody2D>();
        if (_rigidbody == null)
            Debug.LogWarning("No Rigidbody2D component attached");

        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null)
            Debug.LogWarning("No PlayerInput component attached");
        
        _playerDamage = GetComponent<PlayerDamage>();
        if (_playerDamage == null)
            Debug.LogWarning("No PlayerDamage component attached");
        
        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.LogWarning("No Animator component attached");
        
        _positionComposer = camera.GetComponent<CinemachinePositionComposer>();
        if (_positionComposer == null)
            Debug.LogWarning("No PositionComposer component attached to camera");
        
        
        _animator.SetBool(AnimIdleId, true);
        
        _rightWallCheckPosition = rightWallCheck.position;
        _leftWallCheckPosition = leftWallCheck.position;

        // Disable built-in damping (handled by code)
        _rigidbody.linearDamping = 0f;

        RecalculateJumpPhysics();
    }

    void Update()
    {
        //Flip sprite Left and Right depending on where the character is going
        if (_horizontalInput > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            _rightWallCheckPosition = rightWallCheck.position;
            _leftWallCheckPosition = leftWallCheck.position;

        }
        else if (_horizontalInput < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
            _rightWallCheckPosition = leftWallCheck.position;
            _leftWallCheckPosition = rightWallCheck.position;

        }

        //Handle animations depending on the character velocity
        _animator.SetBool(AnimIdleId, isGrounded && Mathf.Abs(_rigidbody.linearVelocity.x) <= Mathf.Epsilon);
        _animator.SetBool(AnimRunningId, isGrounded && Mathf.Abs(_rigidbody.linearVelocity.x) > Mathf.Epsilon);
        _animator.SetBool(AnimJumpingId, !isGrounded && _rigidbody.linearVelocity.y > 0);
        _animator.SetBool(AnimFallingId, !isGrounded && _rigidbody.linearVelocity.y < 0);

        //Handle Camera Offset
        float offsetX = Mathf.SmoothDamp(_positionComposer.TargetOffset.x, _targetOffsetX, ref _camXSmooth, camOffsetAccel);
        float offsetY = Mathf.SmoothDamp(_positionComposer.TargetOffset.y, _targetOffsetY, ref _camYSmooth, camOffsetAccel);
        _positionComposer.TargetOffset = new Vector3(offsetX, offsetY, _positionComposer.TargetOffset.z);
        
        //For debugging
        currentSpeed = new Vector2(_rigidbody.linearVelocity.x,_rigidbody.linearVelocity.y);

        if (isGrounded && !isOnPlatform)
        {
            _safePosCoroutine ??= StartCoroutine(SafePosTime());
        }
        else
        {
            if (_safePosCoroutine != null)
            {
                StopCoroutine(_safePosCoroutine);
                _safePosCoroutine = null;
            }
        }
    }

    void FixedUpdate()
    {
        //Check if grounded using OverlapBox
        Collider2D other = Physics2D.OverlapBox(GroundCheckPosition, groundCheckSize, 0f, groundLayer);
        bool currentlyGrounded = other;
        
        if (currentlyGrounded)
        {
            isGrounded = true;
            isOnPlatform = other.CompareTag("Platform");
        }
        else
        {
            isOnPlatform = false;
        }
        
        //Check if near a wall using OverlapBox
        if (canWallJump)
        {
            bool currentlyNearRightWall = Physics2D.OverlapBox(_rightWallCheckPosition, wallCheckSize, 0f, wallLayer);
            bool currentlyNearLeftWall = Physics2D.OverlapBox(_leftWallCheckPosition, wallCheckSize, 0f, wallLayer);
            
            if (!currentlyGrounded)
            {
                isNearLeftWall = currentlyNearLeftWall;
                isNearRightWall = currentlyNearRightWall;

                //If stuck between two walls cannot wall jump
                if (isNearLeftWall && isNearRightWall)
                {
                    isNearLeftWall = false;
                    isNearRightWall = false;
                }
            }
        }

        //Coyote time
        if (currentlyGrounded)
        {
            if (_coyoteCoroutine != null)
            {
                StopCoroutine(_coyoteCoroutine);
            }
            
            isGrounded = true;
            isNearLeftWall = false;
            isNearRightWall = false;
        }
        else
        {
            if (_wasGrounded)
            {
                if (_coyoteCoroutine != null)
                {
                    StopCoroutine(_coyoteCoroutine);
                }
                _coyoteCoroutine = StartCoroutine(CoyoteTime());
            }
        }
        _wasGrounded = currentlyGrounded;

        //Horizontal Movement
         float targetSpeed = _horizontalInput * maxWalkSpeed;
         _velX = Mathf.SmoothDamp(_rigidbody.linearVelocity.x, targetSpeed, ref _velXSmooth, (_horizontalInput != 0) ? walkAccelerationTime : walkDecelerationTime);
       
        _rigidbody.linearVelocity = new Vector2(_velX, _rigidbody.linearVelocity.y);

        //Jump
        if (_jumpInput)
        {
            if (isGrounded)
            {
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpVelocity);
                isGrounded = false;
            }
            else if (canWallJump)
            {
                if (isNearLeftWall)
                {
                    _rigidbody.linearVelocity = new Vector2(_jumpVelocity * wallJumpMultiplierX, _jumpVelocity * wallJumpMultiplierY);
                    isNearLeftWall = false;
                }

                if (isNearRightWall)
                {
                    _rigidbody.linearVelocity = new Vector2(-(_jumpVelocity * wallJumpMultiplierX), _jumpVelocity * wallJumpMultiplierY);
                    isNearRightWall = false;
                }
            }
            _jumpInput = false;
        }
        
        //Falling
        if (!isGrounded && _rigidbody.linearVelocity.y < 0) 
        {
            if (_rigidbody.linearVelocity.y > terminalSpeed)
            {
                _targetOffsetY = 0;
                _rigidbody.gravityScale *= gravityMultiplier;
            }
            else
            {
                _targetOffsetY = -camOffsetY;
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, terminalSpeed);
            }
        }
        if (isGrounded || isNearLeftWall || isNearRightWall) 
        {
            _targetOffsetY = 0;
            _rigidbody.gravityScale = _baseGravityScale;
        }
        

        //Clamp tiny movement values to 0
        if (Mathf.Abs(_rigidbody.linearVelocity.x) <= 0.0005f)
            _rigidbody.linearVelocity = new Vector2(0, _rigidbody.linearVelocity.y);

        if (Mathf.Abs(_rigidbody.linearVelocity.y) <= 0.0005f)
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0);
    }

    private void OnValidate()
    {
        if (_rigidbody != null)
        {
            RecalculateJumpPhysics();
        }
    }

    public void OnMoveXEvent(InputAction.CallbackContext context)
    {
        float val = context.ReadValue<float>();
        _horizontalInput = (val > 0) ? 1 : (val < 0 ? -1 : 0);
        
        if (_horizontalInput > 0)
        {
            _targetOffsetX = camOffsetX;
        }
        else if (_horizontalInput < 0)
        {
            _targetOffsetX = -camOffsetX;
        }
        else
        {
            _targetOffsetX = 0;
        }
    }

    public void OnJumpEvent(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isGrounded || isNearLeftWall || isNearRightWall)
            {
                _jumpInput = true;
            }
        }

        //Variable jump height
        if (context.canceled && _rigidbody.linearVelocity.y > 0)
        {
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _rigidbody.linearVelocity.y * variableHeightRatio);
        }
    }
    
    private void RecalculateJumpPhysics()
    {
        if (timeToApex <= 0f) return;

        _gravity = (2f * jumpHeight) / (timeToApex * timeToApex);
        _jumpVelocity = _gravity * timeToApex;

        float defaultGravity = Physics2D.gravity.y;
        _baseGravityScale = _gravity / Mathf.Abs(defaultGravity);
        _rigidbody.gravityScale = _baseGravityScale;
        
        currentJumpVelocity = _jumpVelocity;
    }

    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(coyoteTimeWindow);
        isGrounded = false;
    }

    IEnumerator SafePosTime()
    {
        yield return new WaitForSeconds(safePosUpdateTime);
        if (isGrounded)
        {
            _playerDamage.UpdateLastSafePos(transform.position);
        }
        _safePosCoroutine = null;
    }
    
    public void DisableInput()
    {
        _playerInput.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(GroundCheckPosition, groundCheckSize);
        Gizmos.DrawWireCube(_rightWallCheckPosition, wallCheckSize);
        Gizmos.DrawWireCube(_leftWallCheckPosition, wallCheckSize);
    }
}