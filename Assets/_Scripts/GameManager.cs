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

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        InputReceiver.On_P_Input += StopGame;
    }

    

    //Functions that should appear in the Unity Event List of a Toggle must take a booleanvalue as parameter
    public void ToggleCharacterController(bool isToggle)
    {
        characterControllerRef.enabled = isToggle;
    }

    void StopGame()
    {
        if(InputReceiver.Check_If_PausePressed())
        Time.timeScale = 0;
    }
}
