using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAndPlayKindOfFootsteps : MonoBehaviour
{

    FootStepsGetter footStepsGetter;
    AudioSource footStepSource;

    //SO stats
    public PlayerStats playerstats;

    //_From which height of the chart raycast down to detect which kind of footsteps are in floor
    public Transform footStepDetectPosition;

    //filter by mask
    public LayerMask groundMask;

    private void Start()
    {
        //cache the single audiosource responsible for footsteps at start
        footStepSource = GetComponentInChildren<AudioSource>();
    }
    public void DetectFootstepTypeAndPlay()
    {
        RaycastHit hit;

        if (Physics.Raycast(footStepDetectPosition.position, Vector3.down, out hit, playerstats._LineToStepDetectDistance, groundMask))
        {
            footStepsGetter = hit.transform.gameObject.GetComponent<FootStepsGetter>();
            //if(footStepsGetter != null)
            footStepSource.clip = footStepsGetter.stepType.footStepsClip;

            Debug.Log("Got the footstep!");
            PlayFootSteps();
        }


    }
    void PlayFootSteps()
    {
        footStepSource.pitch = Random.Range(0.9f, 1.1f);
        footStepSource.Play();
    }


}
