using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class AudioEventRandomPitch : MonoBehaviour
{
    public float interactionPromptBlendTime;
    //public CanvasGroup backGround;

    [Header("Bools")]
    [SerializeField]
    bool playerIsInTrigger;

    public float pitchMin;
    public float pitchMax;

    public float defaultPitch;

    public AudioSource audioSource;

    public UnityEvent OnAudioHasFinishedPlaying;

    private void Start()
    {
        audioSource.pitch = defaultPitch;
    }

    private void Update()
    {
        if (playerIsInTrigger && InputReceiver.CheckIf_Use_Pressed() && PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_item)
        {
            GenerateRandomPitch();
            PlayAudioSource();
        }

    }

    void PlayAudioSource()
    {
        StartCoroutine(PlayAudioSourceRoutine());
    }

    IEnumerator PlayAudioSourceRoutine()
    {
        audioSource.Play();
        FreezePlayerControls();

        while (audioSource.isPlaying)
        {
            yield return null;
        }
        OnAudioHasFinishedPlaying.Invoke();
        UnFreezePlayerControls();  
    }

    void  GenerateRandomPitch()
    {
        float random =  Random.Range(pitchMin, pitchMax);
        audioSource.pitch = random;
    }

    public void FreezePlayerControls()
    {
        InputReceiver.BlockMovementInput();
    }

    public void UnFreezePlayerControls()
    {
        InputReceiver.UnBlockMovementInputs();
    }



    private void OnTriggerStay(Collider other)
    {
        playerIsInTrigger = true;

    }
}
