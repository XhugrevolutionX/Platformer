using UnityEngine;
using UnityEngine.UI;


public class HealthManager : MonoBehaviour
{
    
    private Image[] _hearts = new Image[3];
    [SerializeField] private Sprite spriteFullHeart;
    [SerializeField] private Sprite spriteHalfHeart;
    [SerializeField] private Sprite spriteEmptyHeart;
    [SerializeField] private PlayerSpawn playerSpawnScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _hearts[i] = transform.GetChild(i).GetComponent<Image>();
        }
    }
    
    public void ActualizeHearts()
    {
        switch (playerSpawnScript.Health)
        {
            case 0:
                _hearts[0].sprite = spriteEmptyHeart;
                break;
            case 1:
                _hearts[0].sprite = spriteHalfHeart;
                break;
            case 2:
                _hearts[0].sprite = spriteFullHeart;
                _hearts[1].sprite = spriteEmptyHeart;
                break;
            case 3:
                _hearts[1].sprite = spriteHalfHeart;
                break;
            case 4:
                _hearts[1].sprite = spriteFullHeart;
                _hearts[2].sprite = spriteEmptyHeart;
                break;
            case 5:
                _hearts[2].sprite = spriteHalfHeart;
                break;
            case 6:
                _hearts[2].sprite = spriteFullHeart;
                break;
        }
    }
}
