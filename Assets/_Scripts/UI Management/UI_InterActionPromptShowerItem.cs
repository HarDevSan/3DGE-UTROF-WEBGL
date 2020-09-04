using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UI_InterActionPromptShowerItem : MonoBehaviour
{
    [Header("InteractionCanvasGroup")]
    public CanvasGroup interactionGroupGeneric;
    public TextMeshProUGUI promptText;

    //private void OnEnable()
    //{
    //    ShowInteractionPrompt();
    //}

    private void Awake()
    {
        InventoryItem.OnItemHasBeenCollected += HideInteractionPrompt;
    }

    public virtual void ShowInteractionPrompt()
    {
        //if(isPlayerInsideTrigger)
        interactionGroupGeneric.alpha = 1;
        promptText.maxVisibleCharacters = promptText.textInfo.characterCount;
    }

    public virtual void HideInteractionPrompt()
    {
        if(interactionGroupGeneric != null)
        interactionGroupGeneric.alpha = 0;
        promptText.maxVisibleCharacters = 0;

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
        //isPlayerInsideTrigger = false;
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
