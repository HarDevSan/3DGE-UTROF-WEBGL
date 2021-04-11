using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class playbacks the audiosources that belong to a scene. E.g.: When a bathroom is loaded, it plays back the bathroom ambience audio sources.
 * This class is needed because Audio Sources that need dynmic behaviour such as play, pause or stop during gameplay need to live in the persistent scene, as 
 * cross referenes between scenes are not possible. Therefore, to able to control which persistent audio source gets played when, based onthe players interactions, 
 * this class uses a method with a switch case on the next scene's name string to play the persistent audio sources that should only play AFTER a new scene is entered.
 * */
public class AudioManager_Scene : MonoBehaviour
{
    //List of per Scene audio clips
    public List<AudioSource> audioSources_BathRoom = new List<AudioSource>();

    //temporary List to pass on

    string currentSceneName = "";
    string lastSceneName = "";

    private void Awake()
    {
        SceneLoader.OnScene_Has_Loaded += PlayAudioSourcesBelongingtoThisScene;    
    
    }

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

    void StopLastSceneSources()
    {
        GetLastSceneName();

    }

    void StopAllAudioSources()
    {
        foreach (AudioSource src in audioSources_BathRoom)
        {
            src.Stop();
        }
    }



}
