using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;


public class TimeLinePlayer : MonoBehaviour
{
    public PlayableDirector playableDirector;

    public void PlayTimeLineAsset()
    {
        playableDirector.Play();
    }
}
