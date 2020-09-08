using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;
using UnityEngine.UI;
using Cinemachine;

public class SceneLoader : MonoBehaviour
{
    public static float loadingprogress;
    public int loadDelay;

    //-------------Events
    public delegate void SceneStartedLoading();
    public static event SceneStartedLoading OnSceneStartedLoading;
    public delegate void SceneIsLoading();
    public static event SceneIsLoading OnSceneIsLoading;
    public delegate void ScenehasLoaded();
    public static event ScenehasLoaded OnScene_Has_Loaded;
    public delegate void ScenehasUnLoaded();
    public static event ScenehasUnLoaded OnScene_Has_UnLoaded;

    [SerializeField]
    string lastSceneName;
    public CinemachineBrain brain;

    private void Awake()
    {
        //-- Dont forget to UNSUBSCRIBE
        //This makes no sense, why is it already subscribed here ?
        SceneTransition.OnPlayerPressedEnterOnSight -= LoadNextScene;
        SceneTransition.OnPlayerPressedEnterOnSight += LoadNextScene;
        SceneTransition.OnPlayerPressedEnterOnSight += UnloadLastScene;
        OnScene_Has_Loaded += PlayerController.SetPlayerToPlayableState;
    }

    private void Start()
    {
        /*There will never be more than 2 scenes in loaded at once. The Persistent scene an one additional scene.
         * Cause of this, we can check which scene is loaded at the start of the game
         * that is NOT named Persistent and then assign its name to the lastSceneName variable.*/
        Scene[] sceneArray =  SceneManager.GetAllScenes();
        foreach(Scene scene in sceneArray)
        {
            if (!scene.name.Equals("Persistent"))
            {
                lastSceneName = scene.name;
            }
        }
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
        Debug.Log("Scene Name param : " + name);

        //Start loading of next scene if the next scene is not already loaded
        if (SceneManager.GetSceneByName(name).isLoaded == false)
        {
            StartCoroutine(WaitForSceneToFinishLoading(name));
        }
    }

    //---------Unloading
    void UnloadLastScene(string notNeeded)
    {
        //Only unload a scene if it's loaded. Otherwise spare the coroutine
        if (SceneManager.GetSceneByName(lastSceneName).isLoaded == true)
        {
            //start unloading of last scene
            StartCoroutine(WaitForSceneToFinishUnloading());
        }

    }

    //----------CoRoutines, managing loading progress and finish 
    IEnumerator WaitForSceneToFinishLoading(string name)
    {
        OnSceneStartedLoading.Invoke();

        brain.enabled = false;
        //Loading the nextScene and thereby create an async operation
        AsyncOperation operation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        //Check if the Async Operation has already been created before doing anything, prevents nullref 
        if (operation != null)
        {

            while (operation.isDone == false)
            {
                OnSceneIsLoading.Invoke();
                loadingprogress = operation.progress;
                yield return null;
            }
        }
        loadingprogress = 0;
        lastSceneName = name;
        yield return new WaitForSeconds(loadDelay);

        OnScene_Has_Loaded.Invoke();
        brain.enabled = true;

    }

    IEnumerator WaitForSceneToFinishUnloading()
    {
        //unLoading the thisScene and thereby create an async operation
        AsyncOperation op = SceneManager.UnloadSceneAsync(lastSceneName);
        if (op != null)
        {
            while (op.isDone == false)
            {
                Debug.Log("IsUnloading");
                yield return null;
            }


        }
        OnScene_Has_UnLoaded.Invoke();
    }
    private void OnDisable()
    {
        SceneTransition.OnPlayerPressedEnterOnSight -= LoadNextScene;
        SceneTransition.OnPlayerPressedEnterOnSight -= UnloadLastScene;
    }
    //private void OnEnable()
    //{
    //    SceneTransition.OnPlayerPressedEnterOnSight += LoadNextScene;
    //    SceneTransition.OnPlayerPressedEnterOnSight += UnloadLastScene;
    //}
}

