using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_InterActionPromptShowerItem : MonoBehaviour
{
    [Header("InteractionCanvasGroup")]
    public CanvasGroup interactionGroupGeneric;

    private void Awake()
    {
        InventoryItem.OnItemHasBeenCollected += HideInteractionPrompt;
    }

    public virtual void ShowInteractionPrompt()
    {
        if (interactionGroupGeneric != null)
            interactionGroupGeneric.alpha = 1;
    }

    public virtual void HideInteractionPrompt()
    {
        if(interactionGroupGeneric != null)
        interactionGroupGeneric.alpha = 0;
    }


    private void OnTriggerStay(Collider other)
    {
        if (PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_item)
        {
            ShowInteractionPrompt();
        }
        else
        {
            HideInteractionPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HideInteractionPrompt();
    }

    private void OnDisable()
    {
        HideInteractionPrompt();
        PlayerController.OnPlayerSeesSomethingInteractable_Item -= ShowInteractionPrompt;
        PlayerController.OnPlayerDoesNotSeeSomehtingInteractable -= HideInteractionPrompt;
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerSeesSomethingInteractable_Item += ShowInteractionPrompt;
        PlayerController.OnPlayerDoesNotSeeSomehtingInteractable += HideInteractionPrompt;
    }

}
