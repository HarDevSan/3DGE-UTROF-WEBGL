﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TextEvent_withInvestigatePrompt : TextEvent_SequentialAndInvestigate
{
    public CanvasGroup buttonGroup;
    public float buttonBlendInTime;

    //Unfortuneately, we cn not inherit and override events
    public delegate void FirstTextHasFinishedPrinting();
    public static event FirstTextHasFinishedPrinting OnFirstTextHasFinishedPrinting;
    public delegate void ButtonsAreBlendedIn();
    public static event ButtonsAreBlendedIn OnButtonsGetBlendedIn;
    public delegate void ButtonsAreBlendedOut();
    public static event ButtonsAreBlendedOut OnButtonsGetBlendedOut;

    public override void Awake()
    {
        base.Awake();
        OnAllTextHasBeenPrinted += StartInvestigateModeTextPrint;
        OnFirstTextHasFinishedPrinting += FreezePlayerControls;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }


    void StartInvestigateModeTextPrint()
    {
   
        if (isTextLeft)
        {
            StartCoroutine(PrintTextAndSelectNextTextForInvestigateRoutine());
        }
        BlendInButtonsAndBlendOutHint();
    }

    void BlendInButtonsAndBlendOutHint()
    {
        StartCoroutine(BlendInButtonsAndBlendOutHintRoutine());
        isDuringInteraction = true;
    }

    void BlendOutButtons()
    {
        StartCoroutine(BlendOutButtonsRoutine());
        isDuringInteraction = false;

    }

    IEnumerator BlendInButtonsAndBlendOutHintRoutine()
    {
        if(OnButtonsGetBlendedIn != null)
        OnButtonsGetBlendedIn.Invoke();

        //while ( buttonGroup.alpha < .999f)
        //{
        //    buttonGroup.alpha = Mathf.Lerp(buttonGroup.alpha, 1, buttonBlendInTime += Time.deltaTime);
        //    yield return null;
        //}
        buttonGroup.alpha = 1;

        yield return null;

    }

    IEnumerator BlendOutButtonsRoutine()
    {
        if (OnButtonsGetBlendedOut != null)
            OnButtonsGetBlendedOut.Invoke();

        //while (buttonGroup.alpha > 0.001 || buttonGroup.alpha == 0)
        //{
        //    buttonGroup.alpha = Mathf.Lerp(buttonGroup.alpha, 0, buttonBlendInTime += Time.deltaTime);
        //    yield return null;
        //}
        buttonGroup.alpha = 0;
        yield return null;
    }

    public void PlayerChoseNo()
    {
        //Clear up all UI elements and reset text to first. Unfreeze PlayerControls. Re enable Brain.
        GameManager.LockCursor();
        ResetTextIndex();
        ResetSelectedTextToFirstText();
        HideAllTextViaAlpha();
        ResetAllTextMaxVisibleChars();
        UnFreezePlayerControls();
        brain.enabled = true;
        BlendOutButtons();

    }

    //BoilerPlate, just in case different functionality will be needed
    public void PlayerChoseYes()
    {
        //Clear up all UI elements and reset text to first. Unfreeze PlayerControls. Re enable Brain.
        GameManager.LockCursor();
        ResetTextIndex();
        ResetSelectedTextToFirstText();
        HideAllTextViaAlpha();
        ResetAllTextMaxVisibleChars();
        UnFreezePlayerControls();
        brain.enabled = true;
        BlendOutButtons();
    }

    IEnumerator PrintTextAndSelectNextTextForInvestigateRoutine()
    {
      
        //Reet the last randomly selected text
        ResetCurrentTextMaxVisibleChar();
        //Select a new randomly selected text
        SelectNextTextInList();

        //Show the current text's via alpha
        ShowCurrentlySelectedTextViaAlpha();

        int visibleCount = 0;
        int totalLength = selectedText.textInfo.characterCount;

        while (visibleCount <= totalLength)
        {
            //Pause Time for while printing

            isPrintingDone = false;
            //Debug.Log("Reached While Loop");
            // Debug.Log("Visible count : " + visibleCount + " VS total of" + selectedText.maxVisibleCharacters);
            selectedText.maxVisibleCharacters = visibleCount;
            visibleCount++;

            //if (pressedUseCount == 0)
            yield return new WaitForSeconds(timeBetweenCharPrint);
            //    else if (pressedUseCount > 0)
            //    {
            //        yield return new WaitForSeconds(timeBetweenCharPrintWhenPlayerPressedUse);
            //    }
        }

        isPrintingDone = true;
      
        yield break;
    }
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    //Maybe not useful for sequential text but keep these anyway
    public override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        //BlendOutButtons();
        //buttonGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        OnAllTextHasBeenPrinted += StartInvestigateModeTextPrint;
        OnFirstTextHasFinishedPrinting += FreezePlayerControls;
    }
    private void OnDisable()
    {
        OnAllTextHasBeenPrinted -= StartInvestigateModeTextPrint;
        OnFirstTextHasFinishedPrinting -= FreezePlayerControls;
    }

}
