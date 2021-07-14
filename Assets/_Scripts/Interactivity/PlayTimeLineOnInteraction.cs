using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class PlayTimeLineOnInteraction : MonoBehaviour
{
    public PlayableDirector timeLineToPlay;

    private void Start()
    {       
        InputReceiver.On_E_Input += PlayTimeLine;
    }


    void PlayTimeLine()
    {
        timeLineToPlay.Play();
    }

    private void OnDisable()
    {
        InputReceiver.On_E_Input -= PlayTimeLine;

    }
}
