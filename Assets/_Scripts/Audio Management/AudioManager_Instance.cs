using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager_Instance : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayAudioSource()
    {
        source.PlayOneShot(source.clip);
    }
}
