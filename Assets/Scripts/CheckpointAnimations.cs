using System;
using UnityEngine;

public class CheckpointAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("Is_active", true);
        }
    }
}
