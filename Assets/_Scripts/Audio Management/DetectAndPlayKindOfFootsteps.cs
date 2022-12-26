using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAndPlayKindOfFootsteps : MonoBehaviour
{

    FootStepsGetter footStepsGetter;
    AudioSource footStepSource;

    public LayerMask groundMask;

    private void Start()
    {
        footStepSource = GetComponentInChildren<AudioSource>();
    }
    public void PlayFootStep()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundMask))
        {
 
            footStepsGetter = hit.transform.GetComponent<FootStepsGetter>();
            footStepSource.clip = footStepsGetter.stepType.footStepsClip;

            Debug.Log("Got the footstep!");
        }
        footStepSource.pitch = Random.Range(0.9f, 1.1f);
        footStepSource.Play();
    
    }

}
