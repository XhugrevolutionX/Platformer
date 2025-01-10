using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class FallingPlatform : MonoBehaviour
{
    
    [SerializeField] private Animator animator;
    private Coroutine _fallingPlatformCoroutine;
    public bool is_dead;
    private Rigidbody2D body;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator.SetBool("Is_falling", false);
        is_dead = false;
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Static;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !animator.GetBool("Is_falling"))
        {
            if (_fallingPlatformCoroutine != null)
            {
                StopCoroutine(_fallingPlatformCoroutine);
            }
            _fallingPlatformCoroutine = StartCoroutine("WaitTime");
        }
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.gameObject.name);
        if(animator.GetBool("Is_falling") && !other.gameObject.CompareTag("Player"))
        {
            is_dead = true;
            animator.SetBool("Is_falling", false);
        }
    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(1f);
        body.bodyType = RigidbodyType2D.Dynamic;
        body.constraints = body.constraints ^ RigidbodyConstraints2D.FreezePositionY;
        animator.SetBool("Is_falling", true);
    }
    
}
