using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuManager : MonoBehaviour
{

    [Header("MainCanvas")]
    public CanvasGroup canvasMain;
    [Header("MainMenuCanvasGroup")]
    public CanvasGroup mainMenuGroup;

    public Animator canvas_MainMenuAnimator;

    public delegate void PlayButtonClicked();
    public static event PlayButtonClicked OnPlayButtonClicked;

    public void OnPlayClicked()
    {
        PlayerController.SetPlayerToUnplayableState();
        Debug.Log("Play button was clicked");
        //canvasMain.alpha = 1;
        canvas_MainMenuAnimator.Play("BlendOutMainMenu");
        canvasMain.blocksRaycasts = false;
        OnPlayButtonClicked.Invoke();
    }

}
