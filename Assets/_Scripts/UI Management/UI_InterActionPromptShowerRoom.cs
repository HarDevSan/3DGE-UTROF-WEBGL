using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UI_InterActionPromptShowerRoom : MonoBehaviour
{
    [Header("InteractionCanvasGroup")]
    public CanvasGroup interactionGroupGeneric;


    private void Awake()
    {
        PlayerController.OnPlayerSeesSomethingInteractable_Room += ShowInteractionPrompt;
        PlayerController.OnPlayerDoesNotSeeSomehtingInteractable += HideInteractionPrompt;
    }


    public virtual void ShowInteractionPrompt()
    {
        interactionGroupGeneric.alpha = 1;

    }

    public virtual void HideInteractionPrompt()
    {
        interactionGroupGeneric.alpha = 0;

    }

    private void OnDisable()
    {
        PlayerController.OnPlayerSeesSomethingInteractable_Room -= ShowInteractionPrompt;
        PlayerController.OnPlayerDoesNotSeeSomehtingInteractable -= HideInteractionPrompt;
    }

}
