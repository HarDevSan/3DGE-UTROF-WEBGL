using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public delegate void FirstSceneFinishedLoading();
    public static event FirstSceneFinishedLoading OnFirstSceneFinishedLoading;
    public delegate void StartMenuFinishedUnloading();
    public static event StartMenuFinishedUnloading OnStartMenuFinishedUnloading;
    public delegate void PersistentSceneFinishedLoading();
    public static event PersistentSceneFinishedLoading OnPersistentSceneFinishedLoading;

    private void Start()
    {
        /*We can't subscribe here as it's async. Need to wait for first scene to load and then we need to load peristent scen
        *otherwise player falls as no collider yet present
        */
        StartMenuManager.OnPlayButtonClicked += LoadFirstScene;
        //StartMenuManager.OnPlayButtonClicked += LoadPersistentScene;

        OnPersistentSceneFinishedLoading += UnloadStartMenuScene;
    }

    void LoadPersistentScene()
    {
        //for convienence, if the persistent scene is already loaded in editor, during run time, this does not matter
        if (SceneManager.GetSceneByName(persistentSceneName).isLoaded == false)
            StartCoroutine(LoadPersistentSceneRoutine());
    }

    void LoadFirstScene()
    {
        //for convienence, if the first scene is already loaded in editor, during run time, this does not matter
        if (SceneManager.GetSceneByName(firstSceneName).isLoaded == false)
            StartCoroutine(LoadFirstSceneandThenPersistentSceneRoutine());
    }


    void UnloadStartMenuScene()
    {
        //only if startmenu scene is loaded the startmenu scene will be unloaded, during run time, this does not matter 
        if (SceneManager.GetSceneByName(startMenuSceneName).isLoaded == true)
            StartCoroutine(UnloadStartMenuRoutine());
    }

    IEnumerator LoadPersistentSceneRoutine()
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
        yield break;
    }

    IEnumerator LoadFirstSceneandThenPersistentSceneRoutine()
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
        LoadPersistentScene();
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

