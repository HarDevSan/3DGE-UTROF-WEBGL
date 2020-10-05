using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;
using Cinemachine;

public class GameManager : MonoBehaviour
{

    public CharacterController characterControllerRef;

    public CinemachineBrain brain;

    public delegate void GameHasBeenPaused();
    public static event GameHasBeenPaused OnGameHasBeenPaused;
    public delegate void GameHasBeenResumed();
    public static event GameHasBeenResumed OnGameHasBeenResumed;

    private void Awake()
    {
        SceneLoader.OnSceneIsLoading += PlayerController.SetPlayerToUnplayableState;
        SceneLoader.OnScene_Has_Loaded += PlayerController.SetPlayerToPlayableState;
        //Can't pause game when coroutines need to run
        InputReceiver.On_P_Input += PauseGame;
        InputReceiver.On_P_Second_Input += ResumeGame;
        InputReceiver.On_P_Second_Input += LockCursor;


        //TextEvent_withInvestigatePrompt.OnButtonsGetBlendedIn += EnableInteractionCanvasBlockRayCasts;
        //TextEvent_withInvestigatePrompt.OnButtonsGetBlendedOut += DisableInteractionCanvasBlockRayCasts;
        CanvasBlendingandBlocking.OnSettingsBlendedIn += UnLockCursor;
        CanvasBlendingandBlocking.OnSettingsBlendedOut += LockCursor;

        CanvasBlendingandBlocking.OnSettingsBlendedIn += UnLockCursor;
        CanvasBlendingandBlocking.OnSettingsBlendedOut += UnLockCursor;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        LockCursor();
        //DisableInteractionCanvasBlockRayCasts();
    }

    private void Update()
    {

        Debug.Log("Character Conroller is enabled : characterControllerRef.enabled");

    }


    void PauseGame()
    {
        Time.timeScale = 0;
        InputReceiver.BlockMovementInput();
        brain.enabled = false;
        OnGameHasBeenPaused.Invoke();
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        InputReceiver.UnBlockMovementInputs();
        brain.enabled = true;
        OnGameHasBeenResumed.Invoke();
    }

    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }

    public static void UnLockCursor()
    {
        Cursor.lockState = CursorLockMode.None;

    }





    private void OnDisable()
    {
        SceneLoader.OnSceneIsLoading -= PlayerController.SetPlayerToUnplayableState;
        SceneLoader.OnScene_Has_Loaded -= PlayerController.SetPlayerToPlayableState;
        //Can't pause game when coroutines needs to run
        //InputReceiver.On_P_Input += PauseGame;
        InputReceiver.On_P_Second_Input -= ResumeGame;
   
    }
}
