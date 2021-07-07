using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateUIElementsOnEnter_Exit : MonoBehaviour
{
    Animator animator;

    public delegate void ReverseAnimCompleted();

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            animator.Play("ExpandPOI");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            animator.Play("Default");
    }

}
