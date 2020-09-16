using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMeshMovement : MonoBehaviour
{
    public Animator animator;
    public string triggerName;

    public void StartMovement()
    {
        animator.SetTrigger(triggerName);
    }
}
