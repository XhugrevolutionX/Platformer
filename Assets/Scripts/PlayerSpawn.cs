using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerSpawn : MonoBehaviour
{
    private Vector3 _spawnPosition;
    
    public Vector3 SpawnPosition => _spawnPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spawnPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("CheckPoint"))
        {
            Debug.Log("CheckPoint Updated");
            _spawnPosition = other.gameObject.transform.position;
        }
    }
}
