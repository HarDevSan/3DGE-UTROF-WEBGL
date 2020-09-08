using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuitPromptShower : MonoBehaviour
{
    // Start is called before the first frame update
    public CanvasGroup quitPRomptGroup;

    private void Awake()
    {
        ReadNote.OnPlayerHasChosenNo += HideQuitPromt;
    }

    public void ShowQuitPrompt()
    {
        quitPRomptGroup.alpha = 1;
    }
    public void HideQuitPromt()
    {
        quitPRomptGroup.alpha = 0;

    }
}
