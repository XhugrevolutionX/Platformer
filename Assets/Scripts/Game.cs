using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{

    // void Start()
    // {
    // }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        Canvas.ForceUpdateCanvases();
    }
}