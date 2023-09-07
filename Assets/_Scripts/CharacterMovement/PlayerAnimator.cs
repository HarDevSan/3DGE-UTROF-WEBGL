using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    //Param to make movement speed more natural by assigning slight randomization in tempo
    public Vector2 RandomSpeedMaxMin;

    private void Start()
    {
        //InputReceiver.On_LeftMouse_Input += SetCombatLayer;
        InputReceiver.On_LeftMouse_Input += SetTriggerStab;
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

    void SetCombatLayer()
    {
        if(animator.GetLayerWeight(1) == 0)
        animator.SetLayerWeight(1, 1);
        else
        animator.SetLayerWeight(1, 0);
    }

    void SetTriggerStab()
    {
        animator.SetTrigger("Trigger_Stab");
    }
}
