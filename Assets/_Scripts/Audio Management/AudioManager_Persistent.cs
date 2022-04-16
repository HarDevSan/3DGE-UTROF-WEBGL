using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager_Persistent : MonoBehaviour
{
    public AudioSource keyCollectedSoundSource;
    public AudioSource trackIntroSource;

    [Header("DoorSoundSource")]
    public AudioSource doorSoundSource;
    [Header("DoorClips")]
    public AudioClip doorOpenClipHeavy;
    public AudioClip doorOpenClipLightweight;
    public AudioClip doorClosingClipHeavy;
    public AudioClip doorClosingClipLightWeight;
    public AudioClip doorIsLockedClipHeavy;
    public AudioClip doorIsLockedClipLightweight;

    public float tempLowerTrackVolume;
    public float tempLowerVolumeSpeed;

    private void Awake()
    {
        SceneTransition.OnHeavyDoorIsLocked += PlayDoorIsLockedSoundHeavy;
        SceneTransition.OnLightWeightDoorIsLocked += PlayDoorIsLockedSoundLightWeight;
        SceneTransition.OnHeavyDoorOpened+= PlayDoorOpenSoundHeavy;
        SceneTransition.OnLightWeightDoorOpened += PlayDoorOpenSoundLightWeight;
        SceneTransition.OnHeavyDoorClosing += PlayDoorCloseSoundHeavy;
        SceneTransition.OnLightWeightDoorClosing += PlayDoorCloseSoundLightWeight;
    }

    private void Start()
    {
        trackIntroSource.Play();
    }
    //Door Locked sounds
    void PlayDoorIsLockedSoundHeavy()
    {
        doorSoundSource.clip = doorIsLockedClipHeavy;
        doorSoundSource.Play();
    }
    void PlayDoorIsLockedSoundLightWeight()
    {
        doorSoundSource.clip = doorIsLockedClipLightweight;
        doorSoundSource.Play();
    }

    //Door Opening sounds
    void PlayDoorOpenSoundHeavy()
    {
        doorSoundSource.clip = doorOpenClipHeavy;
        doorSoundSource.Play();

    }
    void PlayDoorOpenSoundLightWeight()
    {
        doorSoundSource.clip = doorOpenClipLightweight;
        doorSoundSource.Play();

    }

    //Door Closing Sounds
    void PlayDoorCloseSoundHeavy()
    {
        doorSoundSource.clip = doorClosingClipHeavy;
        doorSoundSource.Play();

    }
    void PlayDoorCloseSoundLightWeight()
    {
        doorSoundSource.clip = doorClosingClipLightWeight;
        doorSoundSource.Play();

    }

    /*As the AudioMixer Component is not available in Unity for WebGL, Audio ducking needs to be integrated by hand.
     * Function can listen to an event that is fired once. No need for a Coroutine*/
    public void TemporaryLowerTrackVolume()
    {

        float from = trackIntroSource.volume;
        float to = tempLowerTrackVolume;
        float t = 0f;
        float value = trackIntroSource.volume;

        while(value > tempLowerTrackVolume)
        {
            t += Time.deltaTime + tempLowerVolumeSpeed;
            value = Mathf.Lerp(from, to, t);
            trackIntroSource.volume = value;
        }
    }



}
