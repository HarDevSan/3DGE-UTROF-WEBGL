using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioHintOnActivate : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip clipEnter;
    public AudioClip clipExit;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        audioSource.clip = clipEnter;
        audioSource.Play();
    }
    private void OnTriggerExit(Collider other)
    {
        audioSource.clip = clipExit;
        audioSource.Play();
    }
}
