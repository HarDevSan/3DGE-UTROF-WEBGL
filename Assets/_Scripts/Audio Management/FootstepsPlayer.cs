using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsPlayer : MonoBehaviour
{
    public AudioSource src;

    public Vector2 RandomPitchMaxMin;

    public void PlayFootstepSoundOneShot()
    {
        src.pitch = Random.Range(0.9f, 1.1f);
        src.Play();
    }
}
