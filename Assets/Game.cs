using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    private GameObject _player;
    private Rigidbody2D _rb;
    private PlayerInput _playerInput;

    void Start()
    {
        _player = GameObject.Find("Player");
        _rb = _player.GetComponent<Rigidbody2D>();
        _playerInput = _player.GetComponent<PlayerInput>();
    }
    
    public void RestartGame()
    {
        _player.transform.position = new Vector3(0f, -1.25f, 0f);
        _rb.linearVelocity = Vector3.zero;
        _playerInput.enabled = true;
        
        //Reset / Spawn Diamonds
        
        Time.timeScale = 1f;
    }
}