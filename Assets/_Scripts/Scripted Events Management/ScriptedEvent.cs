using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedEvent : MonoBehaviour
{

    [Header("AudioSources")]
    public AudioSource soundToPlay;

    bool isSoundAlreadyPlayed;

    private void OnTriggerEnter(Collider other)
    {
        if(isSoundAlreadyPlayed == false) 
        soundToPlay.Play();

        isSoundAlreadyPlayed = true;
    }
}
