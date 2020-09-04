using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;
using Cinemachine;

public class SceneTransition : MonoBehaviour
{
    [Header("SceneTRansition Couple")]
    public Transform thisSceneTransition;
    public Transform nextSceneTransition;

    [Header("SceneTransition Time")]
    public int sceneTransitionSeconds;

    [Header("Char Controller Ref")]
    public CharacterController charController;

    [Header("Scene Name to transition to")]
    public string nextSceneName;

    [Header("FreeLookCam Ref")]
    public CinemachineFreeLook cam;

    [Header("Inventory Ref")]
    public Inventory inventorySO;

    [Header("KeyNameIfAny")]
    public string keyName;

    [Header("Tick if this door is locked")]
    public bool isDoorisNeedingKey;

    [Header("Door locked/unlocked state")]
    public bool isDoorUnLocked;

    //---Events
    public delegate void PlayerPressedEnterOnSight(string name);
    public static event PlayerPressedEnterOnSight OnPlayerPressedEnterOnSight;
    //Door is locked Events
    public delegate void HeavyDoorIsLocked();
    public static event HeavyDoorIsLocked OnHeavyDoorIsLocked;
    public delegate void LightWeightDoorClosed();
    public static event LightWeightDoorClosed OnLightWeightDoorIsLocked;
    //Open Door Events
    public delegate void HeavyDoorOpened();
    public static event HeavyDoorOpened OnHeavyDoorOpened;
    public delegate void LightWeightDoorOpened();
    public static event LightWeightDoorOpened OnLightWeightDoorOpened;
    //Closing Door Events
    public delegate void HeavyDoorClosing();
    public static event HeavyDoorClosing OnHeavyDoorClosing;
    public delegate void LightWeightDoorClosing();
    public static event LightWeightDoorClosing OnLightWeightDoorClosing;

    [SerializeField]
    bool isPlayerCanEnterScene;

    [Header("Door Type")]
    public DoorEnum.DoorTypesEnum doorType;

    private void Awake()
    {
        /*Need to UNSUBSCRIBE the event , otherwise it wil lfire again even if the object is disabled
        It's also ok to hanlde scenetransition on playerInput instead of "OnSceneHasLoaed", OnsceneHasLoaded 
        can be used to determin when the playercontrols should be unlocked in the new scene and the blend out of UI loading sceen*/
        PlayerController.OnPlayerSeesSomethingInteractable_Room += UnlockDoorIfPlayerHasKey;
        InputReceiver.On_E_Input += SwitchTransitionActivity;
        InputReceiver.On_E_Input += SpawnPlayerInNewScene;

        SceneLoader.OnScene_Has_Loaded += InvokeDoorClosingSound;
    }

    void InvokeDoorClosingSound()
    {
        if (doorType == DoorEnum.DoorTypesEnum.Heavy)
        {
            Debug.Log("Door INVOKDED");
            OnHeavyDoorClosing.Invoke();
        }
        else if (doorType == DoorEnum.DoorTypesEnum.Lightweight)
        {
            OnLightWeightDoorClosing.Invoke();
        }
    }


    private void Update()
    {
        //When the player raycast hits something interactable and the player has pressed the use key and the door is unlocked
        if (PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_Room && InputReceiver.CheckIf_Use_Pressed() && CheckIfDoorUnLocked())
        {
            //Send door type to audio manager
            if (doorType == DoorEnum.DoorTypesEnum.Heavy)
            {
                OnHeavyDoorOpened.Invoke();
            }
            else if (doorType == DoorEnum.DoorTypesEnum.Lightweight)
            {
                OnLightWeightDoorOpened.Invoke();
            }

            //Invoking player pressed used on sight, spawn player in next scene, switch trigger activity
            OnPlayerPressedEnterOnSight.Invoke(nextSceneName);
            SpawnPlayerInNewScene();
            SwitchTransitionActivity();        
        }
        else if(PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_Room && InputReceiver.CheckIf_Use_Pressed())
        {
            //Send door type to audio manager
            if (doorType == DoorEnum.DoorTypesEnum.Heavy)
            {
                OnHeavyDoorIsLocked.Invoke();
            }
            else if (doorType == DoorEnum.DoorTypesEnum.Lightweight)
            {
                OnLightWeightDoorIsLocked.Invoke();
            }
        }
    }

    public bool CheckIfDoorUnLocked()
    {
        return isDoorUnLocked;
    }

    void UnlockDoor()
    {
        isDoorUnLocked = true;
    }

    void UnlockDoorIfPlayerHasKey()
    {

        if (inventorySO.SearchListFor(keyName))
        {
            UnlockDoor();
        }
        else
        {
        }
    }



    //---Spawning
    void SpawnPlayerInNewScene()
    {
        //Debug.Log("SpawnPlayerInNewScene");

        //Disable charController for the time the it gets offset
        charController.enabled = false;
        //Position the char at the next scenetranstion position  (e.g. behind the door , or a ladder)
        charController.transform.position = nextSceneTransition.position;

        //Finally enable the charcontroller again, makes no sense to do this in th same frame
        charController.enabled = true;
    }

    void SwitchTransitionActivity()
    {
        //Debug.Log("SwitchTriggerCalled");
        thisSceneTransition.gameObject.SetActive(false);
        nextSceneTransition.gameObject.SetActive(true);

    }

    //---Getter/Setters
    public int GetSceneTransitionSeconds()
    {
        return sceneTransitionSeconds;
    }

    //--- Unsubscribe events on deactivation
    private void OnDisable()
    {
        InputReceiver.On_E_Input -= SpawnPlayerInNewScene;
        InputReceiver.On_E_Input -= SwitchTransitionActivity;

        //SceneLoader.OnScene_Has_Loaded -= SpawnPlayerInNewScene;
        //SceneLoader.OnScene_Has_Loaded -= SwitchTransitionActivity;

    }

}
