using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private UnityEvent playerDeath;
    [SerializeField] private UnityEvent playerDamage;
    private Vector3 _spawnPosition;
    [SerializeField] private int health = 6;


    public int Health => health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spawnPosition = transform.position;
        Debug.Log(Time.timeScale);
    }

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("CheckPoint"))
        {
            Debug.Log("CheckPoint Updated");
            _spawnPosition = other.gameObject.transform.position;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("DeathZone"))
        {
            Debug.Log("Player Damaged");
            health -= 1;
            if (health > 0)
            {
                transform.position = _spawnPosition;
            }
            else
            {
                playerDeath.Invoke();
            }

            playerDamage.Invoke();
        }
    }
}
