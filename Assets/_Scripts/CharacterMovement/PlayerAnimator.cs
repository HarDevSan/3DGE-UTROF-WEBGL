using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;

    public Vector2 RandomSpeedMaxMin;

    private void Update()
    {
        PlayWalkingAnimatorStateWhilePlayerMovesForward();
    }

    void PlayWalkingAnimatorStateWhilePlayerMovesForward()
    {
        animator.speed = Random.Range(0.9f, 1.1f);
            animator.SetBool("IsWalking", !PlayerController.isPlayerIdling);
      
    }
}
