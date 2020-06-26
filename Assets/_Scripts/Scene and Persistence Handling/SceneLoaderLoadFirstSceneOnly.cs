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
    bool isPersistentSceneLoaded;

    private void Start()
    {
        StartMenuManager.OnPlayButtonClicked += LoadPersistentScene;
        StartMenuManager.OnPlayButtonClicked += LoadFirstScene;
        StartMenuManager.OnPlayButtonClicked += UnloadStartMenuScene;
    }

    void LoadPersistentScene()
    {
        StartCoroutine(LoadPersistentRoutine());
    }

    void LoadFirstScene()
    {
        StartCoroutine(LoadFirstSceneRoutine());
    }


    void UnloadStartMenuScene()
    {
        //if (isPersistentSceneLoaded)

            StartCoroutine(UnloadStartMenuRoutine());
    }

    IEnumerator LoadPersistentRoutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(persistentSceneName, LoadSceneMode.Additive);

        if (operation != null)
        {

            while (operation.isDone == false)
            {
                yield return null;
            }
        }
        isPersistentSceneLoaded = true;

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

