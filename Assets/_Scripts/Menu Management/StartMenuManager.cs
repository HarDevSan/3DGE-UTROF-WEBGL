using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuManager : MonoBehaviour
{

    [Header("MainCanvas")]
    public CanvasGroup canvasMain;
    [Header("MainMenuCanvasGroup")]
    public CanvasGroup mainMenuGroup;


    public delegate void PlayButtonClicked();
    public static event PlayButtonClicked OnPlayButtonClicked;

    public void OnPlayClicked()
    {
        PlayerController.SetPlayerToUnplayableState();
        Debug.Log("Play button was clicked");
        canvasMain.alpha = 0;
        canvasMain.blocksRaycasts = false;
        OnPlayButtonClicked.Invoke();
    }

}
