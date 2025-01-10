using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject goodGameText;
    [SerializeField] private GameObject badGameText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HideGoodGame();
        HideBadGame();
    } 
    
    public void HideGoodGame()
    {
        goodGameText.SetActive(false);
    }

    public void ShowGoodGame()
    {
        Time.timeScale = 0f;
        goodGameText.SetActive(true);
    }
    
    public void HideBadGame()
    {
        badGameText.SetActive(false);
    }

    public void ShowBadGame()
    {
        Time.timeScale = 0f;
        badGameText.SetActive(true);
    }
}
