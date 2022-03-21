﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;
using Cinemachine;

/* Scene Transition is a binary state switch system similar to a flip flop, but not quite, as there are basically 3 states: A-B-C. It uses Unity internal physics
 * components like a Box Collider, the players proximity to This scene transition visually represented by a door or similar real life objects, can be detected. When the
 * player is not in proximity, it needs to be ensured that there is no trigger active. This is due to the component based architecture used in Unity. Methods will get
 * called on all Game Objectsthat houses THIS script,which would cause multiple scene loading logic to execute simultaniouslysn.
 * 
 * Author: Hardev Sandhu
 */
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
        //InputReceiver.On_E_Input += SwitchTransitionActivity;
        //InputReceiver.On_E_Input += SpawnPlayerInNewScene;

        SceneLoader.OnScene_Has_Loaded += InvokeDoorClosingSound;
    }

    //Fire an event that an audio player can subscribe to and play a sound effect appropriate to the chosen door type
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
        /*When the player raycast hits something interactable and the player has pressed the use key and the door is unlocked, call the events responsible for door opening.
        This won't put a lot of load on the Update loop, cause the respective events only gets executed if the player also hits the use button in one particular frame.
        Of course, the checking will take place every frame, but the overhead for that is negligable in this case, because there are not a lot of other checks running per frame.
        Also doing this every frame is mandatory, as the player at any time can approach a door and use it.*/
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
            //This will switch the collider activity for Outside and Inside, like a flip flop.
            SwitchTransitionActivity();
        }
        else if (PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_Room && InputReceiver.CheckIf_Use_Pressed())
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

    /*------------------------------------------Checks-------------------------------------*/
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

    /*-------------------------------------Spawning------------------------------------------------------*/
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

    /*------------------------------------Scene Transition Activity----------------------------------*/

    void SwitchTransitionActivity()
    {
        //Debug.Log("SwitchTriggerCalled");
        thisSceneTransition.gameObject.SetActive(false);
        nextSceneTransition.gameObject.SetActive(true);

    }

    /*-----------------------------------Getter/Setters-----------------------------------------------*/
    public int GetSceneTransitionSeconds()
    {
        return sceneTransitionSeconds;
    }

    /*------------------------------------Enable/Disable----------------------------------------------*/
    private void OnDisable()
    {
        //InputReceiver.On_E_Input -= SpawnPlayerInNewScene;
        //InputReceiver.On_E_Input -= SwitchTransitionActivity;

    }
    private void OnEnable()
    {
        //InputReceiver.On_E_Input += SpawnPlayerInNewScene;
        //InputReceiver.On_E_Input += SwitchTransitionActivity;

    }

}
