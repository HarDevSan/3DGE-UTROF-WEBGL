﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static float loadingprogress;

    //-------------Events
    public delegate void SceneStartedLoading();
    public static event SceneStartedLoading OnSceneStartedLoading;
    public delegate void SceneIsLoading();
    public static event SceneIsLoading OnSceneIsLoading;
    public delegate void ScenehasLoaded();
    public static event ScenehasLoaded OnSceneHasLoaded;
    public delegate void ScenehasUnLoaded();
    public static event ScenehasUnLoaded OnSceneHasUnLoaded;

    [SerializeField]
    string lastSceneName;

    private void Awake()
    {
        SceneTransition.OnPlayerPressedEnterInTrigger += LoadNextScene;
        SceneTransition.OnPlayerPressedEnterInTrigger += UnloadLastScene;
    }

    private void Start()
    {
        lastSceneName = "Room_1";
    }

    bool CheckIfSceneHasOutdoors()
    {
        if (GameObject.FindWithTag("HasOutDoorScene") != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //-----------Loading
    public void LoadNextScene(string name)
    {


        //Start loading of next scene
        StartCoroutine(WaitForSceneToFinishLoading(name));
        OnSceneStartedLoading.Invoke();

        //check if the scene that needs to unloaded is loaded alread, if not, spare this step               
    }

    //---------Unloading
    void UnloadLastScene(string name)
    {
        //start unloading of last scene
        StartCoroutine(WaitForSceneToFinishUnloading());
    }

    //----------CoRoutines, managing loading progress and finish 
    IEnumerator WaitForSceneToFinishLoading(string name)
    {
        //Loading the nextScene and thereby create an async operation
        AsyncOperation operation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

        //Check if the Async Operation has already been created before doing anything, prevents nullref 
        while (operation.isDone == false)
        {
            //Debug.Log("LoadingInProgress");
            OnSceneIsLoading.Invoke();
            loadingprogress = operation.progress;
            yield return null;
        }
        //Debug.Log("Loading Finished");
        loadingprogress = 0;
        OnSceneHasLoaded.Invoke();
        lastSceneName = name;
        Debug.Log("Scene that will unload next is : " + lastSceneName);
        yield break;
    }

    IEnumerator WaitForSceneToFinishUnloading()
    {
        //unLoading the thisScene and thereby create an async operation
        AsyncOperation op = SceneManager.UnloadSceneAsync(lastSceneName);
        //Debug.Log("Name Of the LAst Scene : " + lastSceneName);

        while (op.isDone == false)
        {
            //Debug.Log("IsUnloading");
            yield return null;
        }
        // Debug.Log("finishedUnloading ");
        yield break;

    }
}

