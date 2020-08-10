using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;

public class SceneLoaderIntro : MonoBehaviour
{
    //REFS
    [Header("PersistentSceneName")]
    public string persistentSceneName;
    [Header("firstSceneName")]
    public string firstSceneName;
    [Header("StartMenuSceneName")]
    public string introSceneName;
    bool isPersistentSceneLoaded;


    private void Start()
    {
        TextEvent_Sequential.OnAllTextHasBeenPrinted += LoadFirstScene;
        TextEvent_Sequential.OnAllTextHasBeenPrinted += LoadPersistentScene;
    }

    void LoadPersistentScene()
    {
        //for convienence, if the persistent scene is already loaded in editor
        if (SceneManager.GetSceneByName(persistentSceneName).isLoaded == false)
            StartCoroutine(LoadPersistentRoutine());
    }

    void LoadFirstScene()
    {
        //for convienence, if the first scene is already loaded in editor
        if (SceneManager.GetSceneByName(firstSceneName).isLoaded == false)
            StartCoroutine(LoadFirstSceneRoutine());
    }


    void UnloadIntroScene()
    {
        //only if startmenu scene is loaded the startmenu scene will be unloaded 
        if (SceneManager.GetSceneByName(introSceneName).isLoaded == true)
            StartCoroutine(UnloadIntroSceneRoutine());
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
        StartCoroutine(UnloadIntroSceneRoutine());

        yield break;
    }


    IEnumerator UnloadIntroSceneRoutine()
    {
        AsyncOperation op = SceneManager.UnloadSceneAsync(introSceneName);

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

