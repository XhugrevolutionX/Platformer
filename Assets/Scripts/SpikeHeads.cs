using UnityEngine;

public class SpikeHeadAnimation : MonoBehaviour
{
    
    [SerializeField] private Animator animator;
    private Vector3 originalPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator.SetBool("Is_rushing", false);
        
        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Is_rushing"))
        {
            transform.localPosition += Vector3.down * (Time.deltaTime * 6f);
        }
        else if(transform.localPosition.y < originalPosition.y)
        {
            transform.localPosition += Vector3.up * (Time.deltaTime * 3f);
        }
        else
        {
            transform.localPosition = originalPosition;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && transform.localPosition == originalPosition)
        {
            animator.SetBool("Is_rushing", true);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        animator.SetBool("Is_rushing", false);
    }
    
}
