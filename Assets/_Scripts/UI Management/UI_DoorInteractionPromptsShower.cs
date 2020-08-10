using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_DoorInteractionPromptsShower : MonoBehaviour
{
    [Header("InteractionCanvasGroup")]
    public CanvasGroup interactionGroupOpenDoor;
    public CanvasGroup interactionGroupLockedDoor;
    public CanvasGroup interactionGroupUnLockedDoor;
    public CanvasGroup interactionGroupBrokenDoor;

    SceneTransition attachedSceneTransition;

    private void Awake()
    {
        PlayerController.OnPlayerSeesSomethingInteractable_Room += ShowInteractionPrompt;
        PlayerController.OnPlayerDoesNotSeeSomehtingInteractable += HideInteractionPrompt;

        attachedSceneTransition = GetComponent<SceneTransition>();
    }


    public  void ShowInteractionPrompt()
    {
        if (attachedSceneTransition.CheckIfDoorNeedAKey())
        {
            interactionGroupLockedDoor.alpha = 1;
            interactionGroupOpenDoor.alpha = 0;
        }
        else
        {
            interactionGroupLockedDoor.alpha = 0;
            interactionGroupOpenDoor.alpha = 1;
        }

    }

    public  void HideInteractionPrompt()
    {

        interactionGroupOpenDoor.alpha = 0;
        interactionGroupLockedDoor.alpha = 0;

    }

    private void OnEnable()
    {
        ShowInteractionPrompt();
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerSeesSomethingInteractable_Room -= ShowInteractionPrompt;
        PlayerController.OnPlayerDoesNotSeeSomehtingInteractable -= HideInteractionPrompt;

        interactionGroupOpenDoor.alpha = 0;
        interactionGroupLockedDoor.alpha = 0;
        interactionGroupUnLockedDoor.alpha = 0;
        interactionGroupBrokenDoor.alpha = 0;
    }




}

