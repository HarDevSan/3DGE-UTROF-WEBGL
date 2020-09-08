﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReadNote : TextEvent_SequentialAndInvestigate
{
    [Header("Note Content Texts List")]
    public List<CanvasGroup> noteTextGroupList;
    public CanvasGroup backGoundImageGrp;
    public Image foreGroundDarkenImage;
    public CanvasGroup buttonReReadYesNoGRP;
    public float blendInBackGroundSpeed;
    public float blendInBackForeGroundSpeed;
    public float backGroundOpacityMax;
    public float foreGroundOpacityMax;
    public float buttonBlendTime;

    /*Disable the class that drives printing before
     Otherwise we have duplicated text*/
    public TextEvent_withInvestigatePrompt parentScript;

    bool isReadingNote;

    public delegate void ReachedEndOfNote();
    public static event ReachedEndOfNote OnReachedEndOfNote;
    public delegate void PlayerHasChosenNo();
    public static event PlayerHasChosenNo OnPlayerHasChosenNo;

    public CanvasGroup selectedTextGroup;

    public override void Awake()
    {
        base.Awake();
        OnReachedEndOfNote += BlendInReReadButtons;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        Debug.Log("IsTextLeft : " + isTextLeft);
        Debug.Log("isReadingNote : " + isReadingNote);

        /*Do not call base.Update in this case, we do not want parent class referenced buttons to blend in after content has been read
         Need to use custom Buttons for re-read note ? Yes/Quit*/
        if (isReadingNote)
        {
            PlayerController.SetPlayerToUnplayableState();
            CheckIfThereIsTextLeft();

            if (InputReceiver.CheckIf_Use_Pressed() && isPrintingDone && PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_item)
            {
                PlayerController.SetPlayerToUnplayableState();
                StartPrintingNoteContent();
            }
            else if (InputReceiver.CheckIf_Quit_Pressed())
            {
                //StopAllCoroutines();
                PlayerChoseNo();
            }
        }
        //Blend In forgreound to darken the notes graphical representation
        if (textIndex == 2 && InputReceiver.CheckIf_Use_Pressed())
        {
            BlendInForeGroundToDarkenImage();
        }
    }
    void BlendInReReadButtons()
    {
        Debug.Log("ReachedEnOfNote");
        StartCoroutine(BlendInReReadButtonsRoutine());
        buttonReReadYesNoGRP.blocksRaycasts = true;
        GameManager.UnLockCursor();
    }
    void BlendOutReReadButtons()
    {
        Debug.Log("ReachedEnOfNote");
        StartCoroutine(BlendOutReReadButtonsRoutine());
        buttonReReadYesNoGRP.blocksRaycasts = false;
    }

    void BlendInForeGroundToDarkenImage()
    {
        StartCoroutine(BlendInForeGroundImageRoutine());
    }
    void BlendOutForeGroundToDarkenImage()
    {
        StartCoroutine(BlendOutForeGroundImageRoutine());
    }


    public void DisableParentScript()
    {
        parentScript.enabled = false;
    }
    public void EnableParentScript()
    {
        parentScript.enabled = true;
    }

    public void PlayerChoseYes()
    {
        GameManager.LockCursor();
        ResetTextIndex();
        textIndex = 1;
        isTextLeft = true;
        ResetSelectedTextToFirstText();
        HideAllTextViaAlpha();
        ResetAllTextMaxVisibleChars();
        /*Not enabing the brain in this case, because the player will read 
         * the note or investigate the object after all text has ben printed*/
        //PlayerController.SetPlayerToUnplayableState();
        //brain.enabled = false;
        BlendOutReReadButtons();
        StartPrintingNoteContent();

    }

    public void PlayerChoseNo()
    {
        isReadingNote = false;
        isTextLeft = true;
        OnPlayerHasChosenNo.Invoke();
        GameManager.LockCursor();
        ResetTextIndex();
        ResetSelectedTextToFirstText();
        HideAllTextViaAlpha();
        ResetAllTextMaxVisibleChars();
        PlayerController.SetPlayerToPlayableState();
        brain.enabled = true;
        BlendOutReReadButtons();
        BlendOutBackGroundImage();
        BlendOutForeGroundToDarkenImage();
        EnableParentScript();

    }

    public void HardRestAllAplhasAndRayCastBlocks()
    {
        
        foreach(CanvasGroup noteGRP in noteTextGroupList)
        {
            noteGRP.alpha = 0;
        }
        backGoundImageGrp.alpha = 0;
        foreGroundDarkenImage.color = new Color(0,0,0,0);
        buttonReReadYesNoGRP.blocksRaycasts = false;
        buttonReReadYesNoGRP.alpha = 0;
    }

    public override void HideAllTextViaAlpha()
    {
        foreach (CanvasGroup textInList in noteTextGroupList)
        {
            textInList.alpha = 0;
        }

    }

    public override void ResetSelectedTextToFirstText()
    {
        selectedTextGroup = noteTextGroupList[0];

    }
    public override void SelectNextTextInList()
    {
        if (textIndex < noteTextGroupList.Count && textIndex >= 0)
        {
            selectedTextGroup = noteTextGroupList[textIndex];
            textIndex++;
        }
    }

    public override void CheckIfThereIsTextLeft()
    {
        if (textIndex < noteTextGroupList.Count)
        {
            isTextLeft = true;
        }
        else
        {

            isTextLeft = false;

        }
    }

    public void ResetSelectedTextGroupToFirstText()
    {

        selectedTextGroup = noteTextGroupList[0];

    }

    public void ResetAllTextsGroupsVisibility()
    {
        foreach (CanvasGroup textInList in noteTextGroupList)
        {
            textInList.alpha = 0;
        }
    }

    public void StartPrintingNoteContent()
    {
        isReadingNote = true;

        if (isTextLeft)
        {
            StartCoroutine(PrintNoteTextRoutine());
        }
        else
        {
            isReadingNote = false;
        }
    }

    public void BlendInBackGroundImage()
    {
        StartCoroutine(BlendInBackGroundImageRoutine());
    }
    public void BlendOutBackGroundImage()
    {
        StartCoroutine(BlendOutBackGroundImageRoutine());
    }

    IEnumerator BlendInForeGroundImageRoutine()
    {
        Color tempCol = new Color(0, 0, 0, foreGroundOpacityMax);

        while (tempCol.a <= foreGroundOpacityMax)
        {
            tempCol.a = Mathf.Lerp(foreGroundDarkenImage.color.a, foreGroundOpacityMax, blendInBackForeGroundSpeed * Time.deltaTime);
            foreGroundDarkenImage.color = tempCol;

            yield return null;
        }

        yield return null;
    }

    IEnumerator BlendOutForeGroundImageRoutine()
    {
        Color tempCol = new Color(0, 0, 0, 0);

        while (tempCol.a <= foreGroundOpacityMax)
        {
            tempCol.a = Mathf.Lerp(foreGroundDarkenImage.color.a, 0, blendInBackForeGroundSpeed * Time.deltaTime);
            foreGroundDarkenImage.color = tempCol;

            yield return null;
        }

        yield return null;
    }

    IEnumerator BlendInReReadButtonsRoutine()
    {
        while (buttonReReadYesNoGRP.alpha <= .99f)
        {
            buttonReReadYesNoGRP.alpha = Mathf.Lerp(buttonReReadYesNoGRP.alpha, 1, buttonBlendTime * Time.deltaTime);

            yield return null;
        }
    }

    IEnumerator BlendOutReReadButtonsRoutine()
    {
        while (buttonReReadYesNoGRP.alpha >= .001f)
        {
            buttonReReadYesNoGRP.alpha = Mathf.Lerp(buttonReReadYesNoGRP.alpha, 0, buttonBlendTime * Time.deltaTime);

            yield return null;
        }
    }

    IEnumerator BlendInBackGroundImageRoutine()
    {
        float t = 0f;
        float toLerpFrom = 0f;
        float toLerpTo = backGroundOpacityMax;
        float lerpValue = 0f;

        while (lerpValue < toLerpTo)
        {
            t += blendInBackGroundSpeed * Time.deltaTime;
            lerpValue = Mathf.Lerp(toLerpFrom, toLerpTo, t);
            backGoundImageGrp.alpha = lerpValue;
            Debug.Log("BlendInRuns");
            yield return null;
        }
    }

    IEnumerator BlendOutBackGroundImageRoutine()
    {
        float t = 0f;
        float toLerpFrom = backGroundOpacityMax;
        float toLerpTo = 0f;
        float lerpValue = 1f;

        while (lerpValue > toLerpTo)
        {
            Debug.Log("LerpValue " + lerpValue);
            t += blendInBackGroundSpeed * Time.deltaTime;
            lerpValue = Mathf.Lerp(toLerpFrom, toLerpTo, t);
            backGoundImageGrp.alpha = lerpValue;
            yield return null;
        }
    }

    IEnumerator PrintNoteTextRoutine()
    {
        ResetSelectedTextGroupToFirstText();
        ResetAllTextsGroupsVisibility();
        SelectNextTextInList();

        while (selectedTextGroup.alpha <= .999f)
        {
            selectedTextGroup.alpha = Mathf.Lerp(selectedTextGroup.alpha, 1, blendInBackGroundSpeed * Time.deltaTime);
            yield return null;
        }
        if(isTextLeft == false)
        OnReachedEndOfNote.Invoke();
    }

    private void OnDisable()
    {
        OnReachedEndOfNote -= BlendInReReadButtons;
        //Hard reset all alphas and raycasts
        HardRestAllAplhasAndRayCastBlocks();

    }

    private void OnEnable()
    {
        OnReachedEndOfNote += BlendInReReadButtons;
    }
}
