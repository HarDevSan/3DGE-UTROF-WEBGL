using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_DoorInteractionPromptsShower : MonoBehaviour
{
    [Header("InteractionCanvasGroup")]
    //public CanvasGroup interactionGroupOpenDoor;
    //public CanvasGroup interactionGroupLockedDoor;
    //public CanvasGroup interactionGroupUnLockedDoor;
    //public CanvasGroup interactionGroupBrokenDoor;

    SceneTransition attachedSceneTransition;

    public Animator InteractionCanvasAnimator;

    private void Awake()
    {
        PlayerController.OnPlayerSeesSomethingInteractable_Room += ShowInteractionPrompt;
        PlayerController.OnPlayerDoesNotSeeSomehtingInteractable += HideInteractionPrompt;

        attachedSceneTransition = GetComponent<SceneTransition>();
    }


    public  void ShowInteractionPrompt()
    {
        if (attachedSceneTransition.CheckIfDoorUnLocked())
        {
            InteractionCanvasAnimator.Play("Blend-In_IconUse");
        }
        else
        {
          
        }

    }

    public  void HideInteractionPrompt()
    {
        InteractionCanvasAnimator.Play("Blend-Out_IconUse");


    }

    private void OnEnable()
    {
        //ShowInteractionPrompt();
    }

    private void OnDisable()
    {
       // HideInteractionPrompt();

        //PlayerController.OnPlayerSeesSomethingInteractable_Room -= ShowInteractionPrompt;
        //PlayerController.OnPlayerDoesNotSeeSomehtingInteractable -= HideInteractionPrompt;

    }




}

