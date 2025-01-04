using UnityEngine;

public class Enemies : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Death");
            other.transform.position = other.gameObject.GetComponent<PlayerSpawn>().SpawnPosition;
        }
    }
}
