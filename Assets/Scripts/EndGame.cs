using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EndGame : MonoBehaviour
{
    
    [SerializeField] private UnityEvent endGameEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        endGameEvent.Invoke();
    }
}
