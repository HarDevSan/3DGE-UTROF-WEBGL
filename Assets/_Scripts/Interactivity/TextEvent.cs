using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEvent : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textToDisplay;

    private void Awake()
    {
        PlayerController.OnPlayerSeesSomethingInteractable_Item += ShowText;
        PlayerController.OnPlayerDoesNotSeeSomehtingInteractable += HideText;
    }

    void ShowText()
    {
        textToDisplay.alpha = 1;
    }

    void HideText()
    {
        textToDisplay.alpha = 0;

    }
}
