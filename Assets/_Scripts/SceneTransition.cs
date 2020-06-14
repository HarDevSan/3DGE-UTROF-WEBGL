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
        InputReceiver.On_E_Input += SpawnPlayerInNewScene;
        InputReceiver.On_E_Input += SwitchTransitionActivity;
    }

    private void Update()
    {

        //When the player raycast hits something interactable and the player has pressed the use key 
        if (PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth && InputReceiver.CheckIf_Use_Pressed())
        {
            Debug.Log("Reached");
            OnPlayerPressedEnterOnSight.Invoke(nextSceneName);
            SpawnPlayerInNewScene();
            SwitchTransitionActivity();
        }
    }

    //---Spawning
    void SpawnPlayerInNewScene()
    {
        charController.enabled = false;
        charController.transform.position = nextSceneTransition.position;
        charController.enabled = true;
    }

    void SwitchTransitionActivity()
    {
        Debug.Log("SwitchTriggerCalled");
        thisSceneTransition.gameObject.SetActive(false);
        nextSceneTransition.gameObject.SetActive(true);

    }

    //---Getter/Setters
    public int GetSceneTransitionSeconds()
    {
        return sceneTransitionSeconds;
    }

}
