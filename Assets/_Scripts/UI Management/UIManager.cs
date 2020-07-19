using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
  
    [Header("InteractionCanvasGroup")]
    public CanvasGroup interactionGroup;

    [Header("Buttons")]
    public Button play;
    public Button credits;
    public Button quit;

    [Header("LoadingScreen")]
    public CanvasGroup loadingScreenGroup;
    public Image loadingBar;



    public Transform playerTransform;
    public Transform triggerTransform;

    private void Awake()
    {
        PlayerController.OnPlayerSeesSomethingInteractable_Room += ShowEnterRoomInteraction;
        PlayerController.OnPlayerDoesNotSeeSomehtingInteractable += HideEnterRoomInteraction;
        SceneLoader.OnSceneStartedLoading += ShowLoadingScreen;
        SceneLoader.OnSceneIsLoading += ShowLoadingScreen;
        SceneLoader.OnSceneIsLoading += UpdateLoadingBar;
        SceneLoader.OnScene_Has_Loaded += HideLoadingBar;
    }

    private void Start()
    {
        loadingScreenGroup.blocksRaycasts = false;
    }

    void ShowLoadingScreen()
    {
        loadingScreenGroup.alpha = 1;
        loadingScreenGroup.blocksRaycasts = true; ;
        InputReceiver.BlockMovementInput();

    }

    void UpdateLoadingBar()
    {
        loadingBar.fillAmount = SceneLoader.loadingprogress;

    }

    void HideLoadingBar()
    {
        loadingScreenGroup.blocksRaycasts = false;
        loadingScreenGroup.alpha = 0;
        InputReceiver.UnBlockMovementInputs();
    }


    public void ShowEnterRoomInteraction()
    {
        interactionGroup.alpha = 1;
        
     
    }

    public void HideEnterRoomInteraction()
    {

        interactionGroup.alpha = 0;

    }
    private void OnDisable()
    {
        PlayerController.OnPlayerSeesSomethingInteractable_Room -= ShowEnterRoomInteraction;
        PlayerController.OnPlayerDoesNotSeeSomehtingInteractable -= HideEnterRoomInteraction;
        SceneLoader.OnSceneStartedLoading += ShowLoadingScreen;
        SceneLoader.OnSceneIsLoading -= ShowLoadingScreen;
        SceneLoader.OnSceneIsLoading -= UpdateLoadingBar;
        SceneLoader.OnScene_Has_Loaded -= HideLoadingBar;
    }

}
