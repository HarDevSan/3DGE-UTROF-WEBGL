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
    public static string lastSceneName;
    public static string thisSceneName;

    private void Awake()
    {
        //-- Dont forget to UNSUBSCRIBE
        //Double Check OnEnable Method to prevent double subscription
        SceneTransition.OnPlayerPressedEnterOnSight += LoadNextScene;
        SceneTransition.OnPlayerPressedEnterOnSight += UnloadLastScene;
        OnScene_Has_Loaded += PlayerController.SetPlayerToPlayableState;
        SceneLoaderIntro.OnFirstSceneHasBeenLoaded += FilterScenesToGetFirstSceneNameAfterIntro;
    }

    private void Start()
    {
        /*There will never be more than 2 scenes in loaded at once. The Persistent scene and one additional scene.
         * Cause of this, we can check which scene is loaded at the start of the game
         * that is NOT named Persistent and then assign its name to the lastSceneName variable.*/
        FilterScenesToGetFirstSceneNameAfterIntro();
    }

    public void FilterScenesToGetFirstSceneNameAfterIntro()
    {
        Scene[] sceneArray = SceneManager.GetAllScenes();

        foreach (Scene scene in sceneArray)
        {
            if (!scene.name.Equals("Persistent"))
            {
                lastSceneName = scene.name;
            }
        }

    }

    //-----------Loading
    public void LoadNextScene(string name)
    {
        Debug.Log("thisSceneName : " + thisSceneName );
        Debug.Log("lastSceneName : " + lastSceneName);


        //Start loading of next scene if the next scene is not already loaded
        if (SceneManager.GetSceneByName(name).isLoaded == false)
        {          
            StartCoroutine(WaitForSceneToFinishLoading(name));
        }
    }

    //---------Unloading
    void UnloadLastScene(string notNeeded) //just to match the signature of the "LoadNextScene" method, so that no new delegate needs to be created
    {
        //Only unload a scene if it's loaded. Otherwise spare the coroutine
        if (SceneManager.GetSceneByName(lastSceneName).isLoaded == true)
        {
            //start unloading of last scene
            StartCoroutine(WaitForSceneToFinishUnloading());
        }

    }

    //----------Coroutines, managing loading progress and finish 
    IEnumerator WaitForSceneToFinishLoading(string name)
    {

        OnSceneStartedLoading.Invoke();
        //Loading the nextScene and thereby create an async operation
        AsyncOperation operation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        //Check if the Async Operation has already been created
        if (operation != null)
        {
            while (operation.isDone == false)
            {
                OnSceneIsLoading.Invoke();
                loadingprogress = operation.progress;
                yield return null;
            }
        }
        //Reset loading progress after async isDone
        loadingprogress = 0;
        //Assign last scene name this scene name
        lastSceneName = name;
        //Set the loaded scene as active
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
        //Equalize the min time of waiting by a delay to unify loading screen duration
        yield return new WaitForSeconds(loadDelay);
        //Re-Tetrahedralize light probes
        LightProbes.Tetrahedralize();
        Debug.Log("SceneHasLoaded");
        //Broadcast that next scene has finished loading
        if (OnScene_Has_Loaded != null)
            OnScene_Has_Loaded.Invoke();

    }

    IEnumerator WaitForSceneToFinishUnloading()
    {
        //UnLoading this scene and thereby create an async operation
        AsyncOperation op = SceneManager.UnloadSceneAsync(lastSceneName);
        if (op != null)
        {
            while (op.isDone == false)
            {
                yield return null;
            }


        }
        //Broadcast that this scene has finished unloading
        if (OnScene_Has_UnLoaded != null)
            OnScene_Has_UnLoaded.Invoke();
    }
    private void OnDisable()
    {
        SceneTransition.OnPlayerPressedEnterOnSight -= LoadNextScene;
        SceneTransition.OnPlayerPressedEnterOnSight -= UnloadLastScene;
    }
    private void OnEnable()
    {
        //this would cause adouble subscription
        //SceneTransition.OnPlayerPressedEnterOnSight += LoadNextScene;
        //SceneTransition.OnPlayerPressedEnterOnSight += UnloadLastScene;
    }
}

