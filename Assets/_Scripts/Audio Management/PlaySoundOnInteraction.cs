using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnInteraction : MonoBehaviour
{
    public Animator audioAnimator;

    private void Start()
    {
        InputReceiver.On_E_Input += PlayAudioSources;
    }


    void PlayAudioSources()
    {

    }
}
