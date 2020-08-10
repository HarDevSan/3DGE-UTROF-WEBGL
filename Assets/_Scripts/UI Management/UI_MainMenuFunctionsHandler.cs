using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;
using UnityEngine.UI;

public class UI_MainMenuFunctionsHandler : MonoBehaviour
{

    [Header("Buttons")]
    public Button play;
    public Button credits;
    public Button quit;


    public Transform playerTransform;
    public Transform triggerTransform;

    private void Awake()
    {

        //SceneLoader.OnSceneStartedLoading += ShowLoadingScreen;
        //SceneLoader.OnSceneIsLoading += ShowLoadingScreen;
      
    }

   

   


    private void OnDisable()
    {
 
        //SceneLoader.OnSceneStartedLoading += ShowLoadingScreen;
        //SceneLoader.OnSceneIsLoading -= ShowLoadingScreen;
        //SceneLoader.OnSceneIsLoading -= UpdateLoadingBar;
        //SceneLoader.OnScene_Has_Loaded -= HideLoadingBar;
    }

}
