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

    public Transform thisTrigger;
    public Transform nextTrigger;
    public int sceneTransitionSeconds;
    public Transform player;

    [Header("SceneCouple")]
    public string nextSceneName;

    [Header("Transforms")]
    public Transform playerTransform;

    public bool isInsideTrigger;

    public delegate void PlayerPressedEnterInTrigger(string name);
    public static event PlayerPressedEnterInTrigger OnPlayerPressedEnterInTrigger;

    public CinemachineFreeLook cam;

    private void Awake()
    {
        SceneLoader.OnSceneHasLoaded += SpawnPlayerInNewScene;
        SceneLoader.OnSceneHasLoaded += SwitchTriggerActivity;
    }

    private void Start()
    {
        //   cam = GetComponent<CinemachineFreeLook>();

    }

    void SpawnPlayerInNewScene()
    {
        //Debug.Log("SpawningPlayerToNewScene");
        //Debug.Log("Name of the next Tigger: " + nextTrigger.name);
        player.position = nextTrigger.position;
        player.forward = -nextTrigger.forward;
        //Make the cam look forward when player enters room
        // isCameraRecentering = true;
    }


    //---Trigger Callbacks -- 
    void SwitchTriggerActivity()
    {
      
            thisTrigger.gameObject.SetActive(false);
        
       
            nextTrigger.gameObject.SetActive(true);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        isInsideTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth && InputReceiver.CheckIf_Use_Pressed())
        {
            //Debug.Log("Player Pressed Enter inside Trigger");
            OnPlayerPressedEnterInTrigger.Invoke(nextSceneName);

        }
        isInsideTrigger = true;

    }

    private void OnTriggerExit(Collider other)
    {

        isInsideTrigger = false;
    }

    //---Getter/Setters

    public string GetTriggerName()
    {
        return gameObject.name;
    }


    public int GetSceneTransitionSeconds()
    {
        return sceneTransitionSeconds;
    }

}
