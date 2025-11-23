using System;
using Unity.VisualScripting;
using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [SerializeField] private float force = 10;
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
        Debug.Log("Collide");
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log(other.name);

            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, transform.up.y * force);
        }
    }
}
