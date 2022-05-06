using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraBrainDisabler : MonoBehaviour
{
    public CinemachineBrain freeLookCamBrain;


    private void Awake()
    {
        SceneLoader.OnSceneStartedLoading += TemporaryDisableBrain;
        SceneLoader.OnScene_Has_Loaded += ReEnableBrain;

    }


    void TemporaryDisableBrain()
    {
        freeLookCamBrain.enabled = false;
    }
        
    void ReEnableBrain()
    {
        freeLookCamBrain.enabled = true;

    }

}
