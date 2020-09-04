using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager_Persistent : MonoBehaviour
{
    public AudioSource keyCollectedSound;
    public AudioSource trackIntro;

    [Header("DoorSoundSource")]
    public AudioSource doorSoundSource;
    [Header("DoorClips")]
    public AudioClip doorOpenClipHeavy;
    public AudioClip doorOpenClipLightweight;
    public AudioClip doorClosingClipHeavy;
    public AudioClip doorClosingClipLightWeight;
    public AudioClip doorIsLockedClipHeavy;
    public AudioClip doorIsLockedClipLightweight;

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
        trackIntro.Play();
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



}
