using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10;
    [SerializeField] private float moveForce = 10;
    [SerializeField] private Animator animator;
    private float _horizontalInput;
    private bool _jumpInput;
    private bool _isGrounded;

    private Coroutine _coyoteTime;
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        if (_rigidbody == null)
        {
            Debug.LogWarning("No rigidbody attached");
        }
        
        animator.SetBool("Is_idle", true);
    }

    void Update()
    {
        //Flip the sprite the way it moves
        if (_horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (_horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        
        //animations
        animator.SetBool("Is_idle", _isGrounded && _rigidbody.linearVelocityX == 0);
        animator.SetBool("Is_running", _isGrounded && _rigidbody.linearVelocityX != 0);
        animator.SetBool("Is_jumping", !_isGrounded && _rigidbody.linearVelocityY > 0);
        animator.SetBool("Is_falling", !_isGrounded && _rigidbody.linearVelocityY < 0);
    }

    void FixedUpdate()
    {
        //Jump
        if (_jumpInput)
        {
            if (_isGrounded)
            {
                Debug.Log("JumpForce Applied");
                _rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                _isGrounded = false;
            }

            _jumpInput = false;
        }

        if (_horizontalInput != 0)
        {
            //Left - Right Movement
            _rigidbody.AddForce(transform.right * (moveForce * _horizontalInput), ForceMode2D.Force);
        }

    }

    // //Old Input 
    // void OnMoveX(InputValue value)
    // {
    //     Debug.Log("OnMove");
    //     if (value.Get<float>() > 0)
    //     {
    //         _horizontalInput = 1;
    //     }
    //     else if (value.Get<float>() < 0)
    //     {
    //         _horizontalInput = -1;
    //     }
    //     else
    //     {
    //         _horizontalInput = 0;
    //     }
    // }
    //
    // void OnJump(InputValue value)
    // {
    //     Debug.Log("OnJump");
    //     _jumpInput = value.isPressed;
    // }
    //
    // void OnJumpRelease(InputValue value)
    // {
    //     Debug.Log("OnJumpRelease");
    //     //Half upward velocity on jump button release
    //     _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _rigidbody.linearVelocity.y * 0.5f);
    // }
    //
    // void OnShoot(InputValue value)
    // {
    //     Debug.Log("OnShoot");
    //     return;
    // }

    //New Input

    public void OnMoveXEvent(InputAction.CallbackContext context)
    {
        Debug.Log("Event OnMove");
        if (context.ReadValue<float>() > 0)
        {
            _horizontalInput = 1;
        }
        else if (context.ReadValue<float>() < 0)
        {
            _horizontalInput = -1;
        }
        else
        {
            _horizontalInput = 0;
        }
    }

    public void OnJumpEvent(InputAction.CallbackContext context)
    {
        //Variable jump height
        if (context.started)
        {
            Debug.Log("Event OnJump");
            //Check if grounded
            if (_isGrounded)
            {
                _jumpInput = true;
            }
        }

        if (context.canceled)
        {
            Debug.Log("Event OnJumpRelease");
            //Half upward velocity on jump button release
             if (_rigidbody.linearVelocityY > 0)
            {
                _rigidbody.linearVelocityY *= 0.5f;
            }
        }
    }

    // public void OnShootEvent(InputAction.CallbackContext context)
    // {
    //     Debug.Log("Event OnShoot");
    //     return;
    // }


    //Grounded Trigger
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            _isGrounded = true;
            _jumpInput = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (_jumpInput)
        {
            _isGrounded = false;
        }
        else
        {
            if (gameObject.activeInHierarchy)
            {
                if (_coyoteTime != null)
                {
                    StopCoroutine(_coyoteTime);
                }

                _coyoteTime = StartCoroutine("CoyoteTime");
            }
        }
    }

    //Coyote Time
    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(0.15f);
        _isGrounded = false;
    }
    
    
    public void DisableInput()
    {
        _playerInput.enabled = false;
    }
}