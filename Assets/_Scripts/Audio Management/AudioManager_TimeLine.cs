using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AudioManager_TimeLine : MonoBehaviour
{
    [Header("initialtimeLineBathroom")]
    public PlayableDirector firstTimeline_BathRoom;

    //List of all non persistent Scene timelines to cycle through
    public List<PlayableDirector> nonPersistetnTimelineList = new List<PlayableDirector>();


    string currentSceneName = "";

    private void Awake()
    {
        SceneLoader.OnScene_Has_Loaded += PlayFirstTimelineBelongingtoThisScene;
    }

    void GetCurrentSceneName()
    {
        currentSceneName = SceneLoader.thisSceneName;
    }


    void PlayFirstTimelineBelongingtoThisScene()
    {
        GetCurrentSceneName();

        switch (currentSceneName)
        {
            case "1stFloor_Reception RestRoom_F":
                PlayTimeLine_Bathroom();
                break;
            default:
                StopAllTimeLines();
                break;
        }
    }


    void PlayTimeLine_Bathroom()
    {
    
            firstTimeline_BathRoom.Play();
        
    }
    void StopAllTimeLines()
    {
        foreach (PlayableDirector pd in nonPersistetnTimelineList)
        {
            pd.Stop();
        }

    }
}
