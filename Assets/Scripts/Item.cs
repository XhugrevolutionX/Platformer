using System;
using UnityEngine;
public class Item : MonoBehaviour
{

    public event Action<Item> OnPicked;
    void Start()
    {
        Activate();
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Deactivate();
            OnPicked?.Invoke(this);
        }
    }
    
    public void Deactivate()
    {
        Destroy(gameObject);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
    
}
