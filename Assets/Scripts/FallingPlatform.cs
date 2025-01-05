using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class FallingPlatform : MonoBehaviour
{
    
    [SerializeField] private Animator animator;
    private Coroutine _fallingPlatformCoroutine;
    public bool is_dead;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator.SetBool("Is_falling", false);
        is_dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Is_falling"))
        {
            transform.localPosition += Vector3.down * (Time.deltaTime * 5f);
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
        if(animator.GetBool("Is_falling") && !other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            //is_dead = true;
            //animator.SetBool("Is_falling", false);
        }
    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("Is_falling", true);
    }
    
}
