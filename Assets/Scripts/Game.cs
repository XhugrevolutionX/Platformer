using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        Canvas.ForceUpdateCanvases();
        // _player.transform.position = new Vector3(-100f, -1f, -10f);
        // _rb.linearVelocity = Vector3.zero;
        // _playerInput.enabled = true;
        //
        // //Reset / Spawn Diamonds
    }
}