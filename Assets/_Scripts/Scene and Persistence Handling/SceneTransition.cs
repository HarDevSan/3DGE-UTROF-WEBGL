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


    //---Events
    public delegate void PlayerPressedEnterOnSight(string name);
    public static event PlayerPressedEnterOnSight OnPlayerPressedEnterOnSight;

    [SerializeField]
    bool isPlayerCanEnterScene;

    private void Awake()
    {
        /*Need to UNSUBSCRIBE the event , otherwise it wil lfire again even if the object is disabled
        It's also ok to hanlde scenetransition on playerInput instead of "OnSceneHasLoaed", OnsceneHasLoaded 
        can be used to determin when the playercontrols should be unlocked in the new scene and the blen out of UI loding sceen*/
        InputReceiver.On_E_Input += SpawnPlayerInNewScene;
        InputReceiver.On_E_Input += SwitchTransitionActivity;

        //SceneLoader.OnScene_Has_Loaded += SpawnPlayerInNewScene;
        //SceneLoader.OnScene_Has_Loaded += SwitchTransitionActivity;
    }

    private void Update()
    {

        //When the player raycast hits something interactable and the player has pressed the use key 
        if (PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_Room && InputReceiver.CheckIf_Use_Pressed())
        {
            //Debug.Log("Reached");
            OnPlayerPressedEnterOnSight.Invoke(nextSceneName);
            SpawnPlayerInNewScene();
            SwitchTransitionActivity();
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
