using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TextEvent_withCollectionPrompt : TextEvent_SequentialAndInvestigate
{
    public CanvasGroup buttonGroup;
    public float buttonBlendInTime;

    //Unfortuneately, we cn not inherit and override events
    public delegate void FirstTextHasFinishedPrinting();
    public static event FirstTextHasFinishedPrinting OnFirstTextHasFinishedPrinting;
    public delegate void ButtonsAreBlendedIn();
    public static event ButtonsAreBlendedIn OnButtonsGetBlendedIn;
    public delegate void ButtonsAreBlendedOut();
    public static event ButtonsAreBlendedOut OnButtonsAreBlendedOutAfterPlayerChoseYes;

    public override void Awake()
    {
        base.Awake();
        OnAllTextHasBeenPrinted += StartInvestigateModeTextPrint;
        InventoryItem.OnItemHasBeenCollected += DisableColliders;

    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    void DisableColliders()
    {
        Collider[] colliders = GetComponents<Collider>();
        foreach(Collider c in colliders)
        {
            c.enabled = false;
        }
    }


    void StartInvestigateModeTextPrint()
    {
        //Only start coroutines if this object is enabled, Unity does not automatically prevent Coroutines from running if an object is disabled
        if (gameObject.activeSelf)
        {
            if (isTextLeft)
            {
                StartCoroutine(PrintTextAndSelectNextTextForInvestigateRoutine());
            }
            BlendInButtonsAndBlendOutHint();
        }
    }

    void BlendInButtonsAndBlendOutHint()
    {
        StartCoroutine(BlendInButtonsAndBlendOutHintRoutine());
        isDuringInteraction = true;
        buttonGroup.blocksRaycasts = true;
    }

    void BlendOutButtonsOnYes()
    {
        StartCoroutine(BlendOutButtonsOnYesRoutine());
        isDuringInteraction = false;
        buttonGroup.blocksRaycasts = false;

    }
    void BlendOutButtonsOnNo()
    {
        StartCoroutine(BlendOutButtonsOnNoRoutine());
        isDuringInteraction = false;
        buttonGroup.blocksRaycasts = false;

    }

    IEnumerator BlendInButtonsAndBlendOutHintRoutine()
    {

        if (OnButtonsGetBlendedIn != null)
            OnButtonsGetBlendedIn.Invoke();

        while (buttonGroup.alpha < .999f)
        {
            buttonGroup.alpha = Mathf.Lerp(buttonGroup.alpha, 1, buttonBlendInTime * Time.deltaTime);
            yield return null;
        }
    }
    IEnumerator BlendOutButtonsOnYesRoutine()
    {
        while (buttonGroup.alpha > 0.001)
        {
            buttonGroup.alpha = Mathf.Lerp(buttonGroup.alpha, 0, buttonBlendInTime * Time.deltaTime);
            yield return null;
        }
        //Fire event for Item to get deactivated
        
        if (OnButtonsAreBlendedOutAfterPlayerChoseYes != null)
            OnButtonsAreBlendedOutAfterPlayerChoseYes.Invoke();
    }
    IEnumerator BlendOutButtonsOnNoRoutine()
    {
        while (buttonGroup.alpha > 0.001)
        {
            buttonGroup.alpha = Mathf.Lerp(buttonGroup.alpha, 0, buttonBlendInTime * Time.deltaTime);
            yield return null;
        }
        //Do not fire event for item to get deactivated
    }

    public void PlayerChoseNo()
    {
        //Clear up all UI elements and reset text to first. Unfreeze PlayerControls. Re enable Brain.
        GameManager.LockCursor();
        ResetTextIndex();
        ResetSelectedTextToFirstText();
        HideAllTextViaAlpha();
        ResetAllTextMaxVisibleChars();
        PlayerController.SetPlayerToPlayableState();
        brain.enabled = true;
        BlendOutButtonsOnNo();

    }

    public void PlayerChoseYes()
    {
        //Clear up all UI elements and reset text to first. Unfreeze PlayerControls. Re enable Brain.
        GameManager.LockCursor();
        ResetTextIndex();
        ResetSelectedTextToFirstText();
        HideAllTextViaAlpha();
        ResetAllTextMaxVisibleChars();
        PlayerController.SetPlayerToPlayableState();
        brain.enabled = true;
        BlendOutButtonsOnYes();
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

            isPrintingDone = false;

            selectedText.maxVisibleCharacters = visibleCount;
            visibleCount++;

            yield return new WaitForSeconds(timeBetweenCharPrint);

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
    }

    private void OnEnable()
    {
        OnAllTextHasBeenPrinted += StartInvestigateModeTextPrint;
    }

    private void OnDisable()
    {
        OnAllTextHasBeenPrinted -= StartInvestigateModeTextPrint;
    }

}
