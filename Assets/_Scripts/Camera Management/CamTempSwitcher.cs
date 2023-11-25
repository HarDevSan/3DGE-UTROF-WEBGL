using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;


public class CamTempSwitcher : MonoBehaviour
{
    public CamStateBroadcaster camStateBroadCasterSO;
    public CinemachineVirtualCamera cam;
    public int camPrioIncresedTimeInSeconds;
    public int prioIncreaseAmount;
    int initalPrio;
    public Volume volume;
    public AudioSource audioSource;
    public AudioClip onToCamSwitchClip;
    public AudioClip onFromCamSwitchClip;


    private void OnTriggerEnter(Collider other)
    {
        camStateBroadCasterSO.SwapCinematicState();
        initalPrio = cam.Priority;
        cam.Priority += cam.Priority += prioIncreaseAmount;
        StartCoroutine(TemporaryBlendCameraAndPostFXVolume());
        audioSource.PlayOneShot(onToCamSwitchClip);
    }

    private void OnTriggerExit(Collider other)
    {
        camStateBroadCasterSO.SwapCinematicState();
        ResetPrio();
        ResetTime();
        DisableThis();
    }

    void SwapCinematicState()
    {
        camStateBroadCasterSO.SwapCinematicState();
    }

    void DisableThis()
    {
        gameObject.SetActive(false);
    }

    void DisableVolume()
    {
        if(volume != null)
        volume.enabled = false;
    }

    void PlayAudioSourceOneShot()
    {
        audioSource.PlayOneShot(onFromCamSwitchClip);

    }

    void ResetPrio()
    {
        cam.Priority = initalPrio;
    }

    void ResetTime()
    {
        camPrioIncresedTimeInSeconds = 0;
    }
    IEnumerator TemporaryBlendCameraAndPostFXVolume()
    {
        while (camPrioIncresedTimeInSeconds > 0)
        {
            yield return new WaitForSeconds(1);
            camPrioIncresedTimeInSeconds -= 1;         
        }
        PlayAudioSourceOneShot();
        DisableVolume();
        ResetPrio();
        SwapCinematicState();
        DisableThis();
        yield return null;
    }
}
