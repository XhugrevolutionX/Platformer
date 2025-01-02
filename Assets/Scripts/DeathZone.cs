using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Death");
            other.transform.position = other.GetComponent<PlayerSpawn>().SpawnPosition;
        }
    }
}
