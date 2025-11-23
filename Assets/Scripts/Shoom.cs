using UnityEngine;

public class Shoom : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float checkSize;
    [SerializeField] private LayerMask playerMask;

    [Header("Random Peek Timing")]
    [SerializeField] private Vector2 peekIntervalRange = new Vector2(2f, 4f);

    [Header("Colliders")]
    [SerializeField] private CapsuleCollider2D basicCollider;
    [SerializeField] private BoxCollider2D hiddenCollider;
    [SerializeField] private BoxCollider2D bounceTrigger;
    
    private Animator _animator;
    private BouncePlatform _bouncePlatform;

    private bool _hide;
    private bool _hidden;
    private float _nextPeekTime;

    private static readonly int HideHash = Animator.StringToHash("Hide");
    private static readonly int PeekHash = Animator.StringToHash("Peek");
    private static readonly int PopHash = Animator.StringToHash("Pop");

    void init()
    {
        _hide = false;
        _hidden = false;
        basicCollider.enabled = true;
        hiddenCollider.enabled = false;
        _bouncePlatform.enabled = false;
        bounceTrigger.enabled = false;
    }
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        _bouncePlatform = GetComponent<BouncePlatform>();

        init();
    }

    void Update()
    {
        if (!_hidden)
        {
            if (_hide)
            {
                _hidden = true;
                basicCollider.enabled = false;
                
                hiddenCollider.enabled = true;
                bounceTrigger.enabled = true;
                _bouncePlatform.enabled = true;
                
                gameObject.layer = LayerMask.NameToLayer("Ground");
                _animator.SetTrigger(HideHash);
                
                ScheduleNextPeek();
            }
        }
        else
        {
            if (!_hide)
            {
                _hidden = false;
                basicCollider.enabled = true;
                
                hiddenCollider.enabled = false;
                bounceTrigger.enabled = false;
                _bouncePlatform.enabled = false;
                
                gameObject.layer = LayerMask.NameToLayer("Death");
                _animator.SetTrigger(PopHash);
            }
            else
            {
                TryPeekWhileHidden();
            }
        }
    }

    void FixedUpdate()
    {
        _hide = Physics2D.OverlapCircle(transform.position, checkSize, playerMask);
    }

    private void TryPeekWhileHidden()
    {
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);

        // only allow in real Hidden state (not during transition)
        if (state.IsName("Hidden") && Time.time >= _nextPeekTime)
        {
            _animator.SetTrigger(PeekHash);
            ScheduleNextPeek();
        }
    }

    private void ScheduleNextPeek()
    {
        _nextPeekTime = Time.time + Random.Range(peekIntervalRange.x, peekIntervalRange.y);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, checkSize);
    }
}
