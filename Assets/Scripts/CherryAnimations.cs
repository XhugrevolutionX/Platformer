using UnityEngine;

public class CherryAnimations : MonoBehaviour
{
    
    [SerializeField] private Animator animator;

    private float _targetTime; 
    private float _animationTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator.SetBool("Is_twitching", false);
        _targetTime = Random.Range(0f, 3f);
        _animationTime = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        _targetTime -= Time.deltaTime;
        if (_targetTime <= 0f)
        {
            animator.SetBool("Is_twitching", true);
        }
        if (animator.GetBool("Is_twitching"))
        {
            _animationTime -= Time.deltaTime;
            if (_animationTime <= 0f)
            {
                animator.SetBool("Is_twitching", false);
                _targetTime = Random.Range(0f, 3f);
                _animationTime = 1f;
            }
        }
    }
}
