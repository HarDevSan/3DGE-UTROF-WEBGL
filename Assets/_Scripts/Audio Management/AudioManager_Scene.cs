using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager_Scene : MonoBehaviour
{
    //List of per Scene audio clips
    public List<AudioSource> audioSources_BathRoom = new List<AudioSource>();

    //temporary List to pass on

    string currentSceneName = "";
    string lastSceneName = "";

    private void Awake()
    {
        SceneLoader.OnScene_Has_Loaded += PlayAudioSourcesBelongingtoThisScene;    }

    void GetCurrentSceneName()
    {
        currentSceneName = SceneLoader.thisSceneName;
    }

    void GetLastSceneName()
    {
        lastSceneName = SceneLoader.lastSceneName;

    }

    void PlayAudioSourcesBelongingtoThisScene()
    {
        GetCurrentSceneName();

        switch (currentSceneName)
        {
            case "1stFloor_Reception RestRoom_F":
                PlayAllAudioSourcesOfScene_Bathroom();
                break;
            default:
                StopAllAudioSources();
                break;
        }
    }


    void PlayAllAudioSourcesOfScene_Bathroom()
    {
        foreach (AudioSource src in audioSources_BathRoom)
        {
            src.Play();
        }
    }
    void StopAllAudioSources()
    {
        foreach (AudioSource src in audioSources_BathRoom)
        {
            src.Stop();
        }
    }



}
