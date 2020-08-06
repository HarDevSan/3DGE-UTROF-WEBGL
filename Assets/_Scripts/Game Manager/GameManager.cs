using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;

public class GameManager : MonoBehaviour
{

    public CharacterController characterControllerRef;

    public static bool isCharControllerEnabled;

    private void Awake()
    {
        SceneLoader.OnSceneIsLoading += SetIsCharControllerEnabledToDisabled;
        SceneLoader.OnScene_Has_Loaded += SetIsCharControllerEnabledToEnabled;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        InputReceiver.On_P_Input += StopGame;
        isCharControllerEnabled = true;
        LockCursor();
        
    }

    private void Update()
    {

        characterControllerRef.enabled = isCharControllerEnabled;
        Debug.Log("Character Conroller is enabled : characterControllerRef.enabled");
    }

    public static void SetIsCharControllerEnabledToEnabled()
    {
        isCharControllerEnabled = true;
        InputReceiver.isMovementInput = true;
    }

    public static void SetIsCharControllerEnabledToDisabled()
    {
        isCharControllerEnabled = false;
        InputReceiver.isMovementInput = false;
    }

    //Functions that should appear in the Unity Event List of a Toggle must take a booleanvalue as parameter


    void StopGame()
    {
        if(InputReceiver.Check_If_PausePressed())
        Time.timeScale = 0;
    }

    public static  void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }

    public static void UnLockCursor()
    {
        Cursor.lockState = CursorLockMode.None;

    }

}
