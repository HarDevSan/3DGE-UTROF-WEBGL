using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;


public class TextEvent_SequentialAndInvestigate : MonoBehaviour
{
    /* List of texts representing fragments or sentences that will be displaed on player 
     * investigation, can be randoml selected from. Should be tied to the item/prop this 
     * TextEvent script is attached to*/
    [Header("Texts List")]
    public List<TextMeshProUGUI> textListToDisplay;
    public List<TextMeshProUGUI> investigateTextList;

    [Header("Bools")]
    [SerializeField]
    public bool playerIsInTrigger;
    [SerializeField]
    public bool isUserInvestigating;
    [SerializeField]
    public bool isPrintingDone;
    [SerializeField]
    public bool isTextLeft;

    //Time between each char that gets printed on the Canvas
    [Header("PrintTime")]
    public float defaultTimeBetweenCharPrint;
    public float timeBetweenCharPrint;
    public float timeBetweenCharPrintWhenPlayerPressedUse;

    public int textIndex;

    //global var holding a random textObject
    [Header("Currentl selected text")]
    [SerializeField]
    public TextMeshProUGUI selectedText;

    [Header("Cinemachine Brain Ref")]
    public Cinemachine.CinemachineBrain brain;



    public delegate void AllTextHasBeenPrinted();
    public static event AllTextHasBeenPrinted OnAllTextHasBeenPrinted;
    public delegate void EachTextHasBeenPrinted();
    public static event EachTextHasBeenPrinted OnEachTextHasBeenPrinted;
    public delegate void FirstTextHasBeenPrinted();
    public static event FirstTextHasBeenPrinted OnFirstTextHasBeenPrinted;

    public UnityEvent OnAllTextHasBeenPrintedAndReadyToCollect;


    bool isButtonsVisible;
    public bool isDuringInteraction;

    public virtual void Awake()
    {
        //Init
        ResetAllTextMaxVisibleChars();
        OnFirstTextHasBeenPrinted += PlayerController.SetPlayerToUnplayableState;
    }

    public virtual void Start()
    {

        isPrintingDone = true;
        isTextLeft = true;
 
    }

    public virtual void Update()
    {
        CheckIfThereIsTextLeft();

        Debug.Log(isTextLeft);
        if (InputReceiver.CheckIf_Use_Pressed() && isPrintingDone && PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_item)
        {
            PlayerController.SetPlayerToUnplayableState();
            PrintNextTextAndInvokeAllHasBeenPrintedIfNot();
        }

    }

    public void ShowCurrentlySelectedTextViaAlpha()
    {
        selectedText.alpha = 1;
    }

    public void HideCurrentlySelectedTextViaAlpha()
    {
        selectedText.alpha = 0;

    }

    public void PrintNextTextAndInvokeAllHasBeenPrintedIfNot()
    {

        if (isTextLeft)
        {
            isDuringInteraction = true;

            brain.enabled = false;
            StartCoroutine(PrintTextAndSelectNextTextWhenDoneRoutine());
        }
        else
        {

            if(OnAllTextHasBeenPrinted != null)
            OnAllTextHasBeenPrinted.Invoke();
            UnlockCursorViaGameManager();
        }
    }

    public void UnlockCursorViaGameManager()
    {
        GameManager.UnLockCursor();

    }

    public void HideAllTextViaAlpha()
    {
        foreach (TextMeshProUGUI textInList in textListToDisplay)
        {
            textInList.alpha = 0;
        }

    }

    public virtual void CheckIfThereIsTextLeft()
    {
        if (textIndex < textListToDisplay.Count)
        {
            isTextLeft = true;
        }
        else
        {

            isTextLeft = false;

        }
    }

    public void SelectFirstTextInList()
    {
        selectedText = textListToDisplay[0];
    }

    public virtual void SelectNextTextInList()
    {
        if (textIndex < textListToDisplay.Count && textIndex >= 0)
        {
            selectedText = textListToDisplay[textIndex];
            textIndex++;
        }
    }

    public void SelectNextTextInInvestigateList()
    {
        if (textIndex < investigateTextList.Count && textIndex >= 0)
        {
            selectedText = investigateTextList[textIndex];
            textIndex++;
        }
    }

    public void ResetCurrentTextMaxVisibleChar()
    {
        selectedText.maxVisibleCharacters = 0;
    }


    public virtual void ResetAllTextMaxVisibleChars()
    {
        foreach (TextMeshProUGUI textInList in textListToDisplay)
        {
            textInList.maxVisibleCharacters = 0;
        }
    }

    public void ResetTextIndex()
    {
        textIndex = 0;

    }

    public void ResetSelectedTextToFirstText()
    {

        selectedText = textListToDisplay[0];

    }

    IEnumerator PrintTextAndSelectNextTextWhenDoneRoutine()
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
        //Invoke FirstTextHasBeenPrinted after first selected text has finished printing
        if (textIndex == 1)
        {
            if(OnFirstTextHasBeenPrinted != null)
            OnFirstTextHasBeenPrinted.Invoke();
        }


        yield break;
    }


    public virtual void OnTriggerEnter(Collider other)
    {
        SelectFirstTextInList();
        ShowCurrentlySelectedTextViaAlpha();
        playerIsInTrigger = true;

    }

    //Maybe not useful for sequential text but keep these anyway
    public virtual void OnTriggerStay(Collider other)
    {

    }

    public virtual void OnTriggerExit(Collider other)
    {
        //Reset all elements whenplayer leaves trigger, unlock cursor, reset text to first
        GameManager.LockCursor();
        ResetTextIndex();
        ResetSelectedTextToFirstText();
        HideAllTextViaAlpha();
        ResetAllTextMaxVisibleChars();
        playerIsInTrigger = false;
    }

    private void OnDisable()
    {
        OnFirstTextHasBeenPrinted -= PlayerController.SetPlayerToUnplayableState;

    }

    private void OnEnable()
    {
        OnFirstTextHasBeenPrinted += PlayerController.SetPlayerToUnplayableState;

    }
}
