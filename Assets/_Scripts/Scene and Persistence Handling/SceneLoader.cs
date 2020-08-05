using System.Collections;
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

    private void Awake()
    {
        //-- Dont forget to UNSUBSCRIBE
        SceneTransition.OnPlayerPressedEnterOnSight += LoadNextScene;
        SceneTransition.OnPlayerPressedEnterOnSight += UnloadLastScene;
    }

    private void Start()
    {
        lastSceneName = GetActiveSceneName();
    }

    string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene().name;

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
        //Start loading of next scene if the next scene is not already loaded
        if (SceneManager.GetSceneByName(name).isLoaded == false)
        {
            OnSceneStartedLoading.Invoke();
        StartCoroutine(WaitForSceneToFinishLoading(name));
            //check if the scene that needs to unloaded is loaded alread, if not, spare this step  
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
        //Loading the nextScene and thereby create an async operation
        AsyncOperation operation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        //Check if the Async Operation has already been created before doing anything, prevents nullref 
        if (operation != null)
        {

            while (operation.isDone == false)
            {
                //Debug.Log("LoadingInProgress");
                OnSceneIsLoading.Invoke();
                loadingprogress = operation.progress;
                yield return null;
            }
        }
        //Debug.Log("Loading Finished");
        loadingprogress = 0;
        lastSceneName = name;

        OnScene_Has_Loaded.Invoke();
        //Debug.Log("OnScene_Has_Loaded.Invoke");
        // Debug.Log("Scene that will unload next is : " + lastSceneName);
        yield return new WaitForSeconds(loadDelay);
    }

    IEnumerator WaitForSceneToFinishUnloading()
    {
        //unLoading the thisScene and thereby create an async operation
        AsyncOperation op = SceneManager.UnloadSceneAsync(lastSceneName);
        //Debug.Log("Name Of the LAst Scene : " + lastSceneName);
        if (op != null)
        {
            while (op.isDone == false)
            {
                //Debug.Log("IsUnloading");
                yield return null;
            }


        }
        // Debug.Log("finishedUnloading ");
       OnScene_Has_UnLoaded.Invoke();
       //Debug.Log("OnSceneHasUnloaded was invoked");
        yield break;
    }
    private void OnDisable()
    {
        SceneTransition.OnPlayerPressedEnterOnSight -= LoadNextScene;
        SceneTransition.OnPlayerPressedEnterOnSight -= UnloadLastScene;
    }
}

