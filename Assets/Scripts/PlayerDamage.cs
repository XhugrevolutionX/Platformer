using Unity.VisualScripting;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private Vector2 lastSafePos;

    public void UpdateLastSafePos(Vector2 LastSafePos)
    {
        lastSafePos = LastSafePos;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Damage"))
        {
            
        }
        if (other.gameObject.CompareTag("FallDamage"))
        {
            transform.position = lastSafePos;
        }
    }
}
