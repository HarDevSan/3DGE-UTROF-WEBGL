using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;

public class SceneLoaderLoadFirstSceneOnly : MonoBehaviour
{

    //REFS
    public string firstSceneName;

    private void Start()
    {
        UIManager.OnPlayButtonClicked += LoadFirstScene;
        //SceneLoader.OnScene_Has_Loaded += UnloadTheFirstEverScene;

    }

    void LoadFirstScene()
    {
        SceneManager.LoadSceneAsync(firstSceneName, LoadSceneMode.Additive);
    }






}

