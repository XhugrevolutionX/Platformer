using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        Canvas.ForceUpdateCanvases();
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}