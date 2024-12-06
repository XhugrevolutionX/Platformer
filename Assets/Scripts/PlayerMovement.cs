using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float moveForce;
    private float _horizontalInput;
    private bool _jumpInput;
    private bool _isGrounded;

    private Rigidbody2D _rigidbody;
    private float _rbGravity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        if (_rigidbody == null)
        {
            Debug.LogWarning("No rigidbody attached");
        }

        _rbGravity = _rigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        //Jump
        if (_jumpInput)
        {
            //Check if grounded
            if (_isGrounded)
            {
                _rigidbody.AddRelativeForce(transform.up * jumpForce, ForceMode2D.Impulse);
            }

            _jumpInput = false;
        }

        //Left - Right Movement
        _rigidbody.AddRelativeForce(transform.right * (moveForce * _horizontalInput), ForceMode2D.Force);

        //Flip the sprite the way it moves
        if (_horizontalInput > 0)
        {
            transform.localScale = new Vector3(-0.75f, 0.75f, 0.75f);
        }
        else if (_horizontalInput < 0)
        {
            transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        }
    }

    void OnMoveX(InputValue value)
    {
        Debug.Log("OnMove");
        _horizontalInput = value.Get<float>();
    }

    void OnJump(InputValue value)
    {
        Debug.Log("OnJump");
        _jumpInput = value.isPressed;
    }

    void OnJumpRelease(InputValue value)
    {
        Debug.Log("OnJumpRelease");
        //Half upward velocity on jump button release
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _rigidbody.linearVelocity.y * 0.5f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            StartCoroutine(nameof(CoyoteTime));
        }
    }

    IEnumerator CoyoteTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.15f);
            _isGrounded = false;
            StopCoroutine(nameof(CoyoteTime));
        }
    }

}