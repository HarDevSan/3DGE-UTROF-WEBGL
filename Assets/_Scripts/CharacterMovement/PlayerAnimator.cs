using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;

    public Vector2 RandomSpeedMaxMin;

    private void Start()
    {

    }

    private void Update()
    {
        //Constantly set vector movements to control blend tree
        AnimatorSetFloats();
    }

    void AnimatorSetFloats()
    {
        animator.SetFloat("Forward", InputReceiver.movementInput.y);
        animator.SetFloat("Sideways", InputReceiver.movementInput.x);
    }

}
