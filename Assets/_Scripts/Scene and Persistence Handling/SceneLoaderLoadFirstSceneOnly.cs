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
    [Header("PersistentSceneName")]
    public string persistentSceneName;
    [Header("firstSceneName")]
    public string firstSceneName;
    [Header("StartMenuSceneName")]
    public string startMenuSceneName;
    public static bool isPersistentSceneLoaded;
    public static bool isFirstSceneLoaded;
    public int delayInSecondsUntilLoaded;


    public delegate void FirstSceneFinishedLoading();
    public static event FirstSceneFinishedLoading OnFirstSceneFinishedLoading;
    public delegate void StartMenuFinishedUnloading();
    public static event StartMenuFinishedUnloading OnStartMenuFinishedUnloading;
    public delegate void PersistentSceneFinishedLoading();
    public static event PersistentSceneFinishedLoading OnPersistentSceneFinishedLoading;

    private void Start()
    {
        //We can't subscribe here as it's async. Need to wait for Persistent loaded to load first scene
        //StartMenuManager.OnPlayButtonClicked += LoadFirstScene;
        StartMenuManager.OnPlayButtonClicked += LoadPersistentScene;

        OnFirstSceneFinishedLoading += UnloadStartMenuScene;
    }

    void LoadPersistentScene()
    {
        //for convienence, if the persistent scene is already loaded in editor, during run time, this does not matter
        if (SceneManager.GetSceneByName(persistentSceneName).isLoaded == false)
            StartCoroutine(LoadPersistentAndThenFirstSceneRoutine());
    }

    void LoadFirstScene()
    {
        //for convienence, if the first scene is already loaded in editor, during run time, this does not matter
        if (SceneManager.GetSceneByName(firstSceneName).isLoaded == false)
            StartCoroutine(LoadFirstSceneRoutine());
    }


    void UnloadStartMenuScene()
    {
        //only if startmenu scene is loaded the startmenu scene will be unloaded, during run time, this does not matter 
        if (SceneManager.GetSceneByName(startMenuSceneName).isLoaded == true)
            StartCoroutine(UnloadStartMenuRoutine());
    }

    IEnumerator LoadPersistentAndThenFirstSceneRoutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(persistentSceneName, LoadSceneMode.Additive);

        if (operation != null)
        {

            while (operation.isDone == false)
            {
                yield return null;
            }
        }
        if (OnPersistentSceneFinishedLoading != null)
            OnPersistentSceneFinishedLoading.Invoke();
        isPersistentSceneLoaded = true;
        LoadFirstScene();
        yield break;
    }

    IEnumerator LoadFirstSceneRoutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(firstSceneName, LoadSceneMode.Additive);

        if (operation != null)
        {

            while (operation.isDone == false)
            {
                yield return null;
            }
        }
        if (OnFirstSceneFinishedLoading != null)
        {

            OnFirstSceneFinishedLoading.Invoke();
        }
        isFirstSceneLoaded = true;
        yield break;
    }


    IEnumerator UnloadStartMenuRoutine()
    {

        AsyncOperation op = SceneManager.UnloadSceneAsync(startMenuSceneName);

        if (op != null)
        {
            while (op.isDone == false)
            {
                yield return null;
            }


        }
        yield break;
    }



}

