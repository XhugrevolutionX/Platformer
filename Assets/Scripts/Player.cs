using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _moveForce;
    private CircleCollider2D _groundCollider;
    private float _horizontalInput;
    private bool _jumpInput;

    private Rigidbody2D _rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _groundCollider = transform.GetChild(0).GetComponent<CircleCollider2D>();
        if (_rigidbody == null)
        {
            Debug.LogWarning("No rigidbody attached");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_jumpInput)
        {
            _rigidbody.AddRelativeForce(transform.up * _jumpForce);
            _jumpInput = false;
        }

        _rigidbody.AddRelativeForce(transform.right * (_moveForce * _horizontalInput));
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
}