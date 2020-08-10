using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShowLoadingScreenAndProgress : MonoBehaviour
{
    [Header("LoadingScreen")]
    public CanvasGroup loadingScreenGroup;
    public Image loadingBar;

    private void Awake()
    {
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

 

}
