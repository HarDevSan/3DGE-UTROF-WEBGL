using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAndPlayKindOfFootsteps : MonoBehaviour
{

    FootStepsGetter footStepsGetter;
    AudioSource footStepSource;

    [Range(0, 1)]
    public float MinPitch;
    [Range(0, 1)]
    public float MaxPitch;

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


    public void DetectFootstepTypeAndReverbTypeAndPlay()
    {
        RaycastHit hit;

        if (Physics.Raycast(footStepDetectPosition.position, Vector3.down, out hit, playerstats._LineToStepDetectDistance, groundMask))
        {
            footStepsGetter = hit.transform.gameObject.GetComponent<FootStepsGetter>();
            footStepSource.clip = footStepsGetter.stepType.footStepsClip;

            PlayFootSteps();
        }


    }
    void PlayFootSteps()
    {
        footStepSource.pitch = Random.Range(MinPitch, MaxPitch);
        footStepSource.Play();
    }


}
