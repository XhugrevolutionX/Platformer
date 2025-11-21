using System.Collections;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPhysics : MonoBehaviour
{
    [Header("Movement Settings")] 
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float accelTime = 0.2f;
    [SerializeField] private float decelTime = 0.3f;
  
    [Header("Jump Settings")] 
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float timeToApex = 0.4f;
    [SerializeField] private float variableHeightRatio = 0.5f;
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private float terminalSpeed = -5f;
    
    [Header("References")] 
    [SerializeField] private Animator animator;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    [Header("Debug")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float currentJumpVelocity;
    [SerializeField] private bool isGrounded;

    private float _horizontalInput;
    private bool _jumpInput;
    private bool _rollInput;
    private bool _wasGrounded;
    private Coroutine _coyoteTime;
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;

    // movement smoothing
    private float velXSmooth;

    // derived jump values
    private float _gravity;
    private float _baseGravityScale;
    private float _jumpVelocity;

    // Position of the ground check (pivot is already at feet)
    private Vector2 GroundCheckPosition => (Vector2)transform.position;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();

        if (_rigidbody == null)
            Debug.LogWarning("No Rigidbody2D component attached");

        if (_playerInput == null)
            Debug.LogWarning("No PlayerInput component attached");
        
        animator.SetBool("Is_idle", true);

        // Disable built-in damping (handled by code)
        _rigidbody.linearDamping = 0f;

        RecalculateJumpPhysics();
    }

    void Update()
    {
        //Flip sprite Left and Right depending on where the character is going
        if (_horizontalInput > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (_horizontalInput < 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);

        //Handle animations depending on the character velocity
        animator.SetBool("Is_idle", isGrounded && Mathf.Abs(_rigidbody.linearVelocity.x) <= Mathf.Epsilon);
        animator.SetBool("Is_running", isGrounded && Mathf.Abs(_rigidbody.linearVelocity.x) > Mathf.Epsilon);
        animator.SetBool("Is_jumping", !isGrounded && _rigidbody.linearVelocity.y > 0);
        animator.SetBool("Is_falling", !isGrounded && _rigidbody.linearVelocity.y < 0);

        //For debugging
        currentSpeed = Mathf.Abs(_rigidbody.linearVelocity.x);
    }

    void FixedUpdate()
    {
        // Check if grounded using OverlapBox at the pivot (feet)
        bool currentlyGrounded = Physics2D.OverlapBox(GroundCheckPosition, groundCheckSize, 0f, groundLayer);

        // Handle coyote time
        if (currentlyGrounded)
        {
            if (_coyoteTime != null)
            {
                StopCoroutine(_coyoteTime);
            }
            isGrounded = true;
        }
        else
        {
            if (_wasGrounded)
            {
                if (_coyoteTime != null)
                {
                    StopCoroutine(_coyoteTime);
                }
                _coyoteTime = StartCoroutine(CoyoteTime());
            }
        }
        _wasGrounded = currentlyGrounded;

        //Horizontal Movement
        float targetSpeed = _horizontalInput * maxSpeed;
        float velX = Mathf.SmoothDamp(_rigidbody.linearVelocity.x, targetSpeed, ref velXSmooth, (_horizontalInput != 0) ? accelTime : decelTime);

        _rigidbody.linearVelocity = new Vector2(velX, _rigidbody.linearVelocity.y);

        //Jump
        if (_jumpInput)
        {
            if (isGrounded)
            {
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpVelocity);
                isGrounded = false;
            }

            _jumpInput = false;
        }
        
        //Falling
        if (!isGrounded && _rigidbody.linearVelocity.y < 0) 
        {
            if (_rigidbody.linearVelocity.y > terminalSpeed)
            {
                _rigidbody.gravityScale *= gravityMultiplier;
            }
            else
            {
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, terminalSpeed);
            }
        }
        if (isGrounded) 
        {
            _rigidbody.gravityScale = _baseGravityScale;
        }
        

        // Clamp tiny movement values to 0
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
    }

    public void OnJumpEvent(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            _jumpInput = true;
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
        yield return new WaitForSeconds(0.15f);
        isGrounded = false;
    }

    public void DisableInput()
    {
        _playerInput.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(GroundCheckPosition, groundCheckSize);
    }
}