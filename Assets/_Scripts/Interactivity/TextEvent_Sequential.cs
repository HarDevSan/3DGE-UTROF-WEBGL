using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextEvent_Sequential : MonoBehaviour
{
    /* List of texts representing fragments or sentences that will be displaed on player 
     * investigation, can be randoml selected from. Should be tied to the item/prop this 
     * TextEvent script is attached to*/
    public List<TextMeshProUGUI> textListToDisplay;

    //Serialized fields for check in inspector
    [Header("Bools")]
    [SerializeField]
    bool playerIsInTrigger;
    [SerializeField]
    bool isUserInvestigating;
    [SerializeField]
    bool isPrintingDone;
    [SerializeField]
    bool isTextLeft;

    //Time between each char that gets printed on the Canvas
    [Header("PrintTime")]
    public float defaultTimeBetweenCharPrint;
    public float timeBetweenCharPrint;
    public float timeBetweenCharPrintWhenPlayerPressedUse;

    public int textIndex;

    [SerializeField]
    int pressedUseCount;

    //global var holding a random textObject, serialized for debugging
    [Header("Currently selected text")]
    [SerializeField]
    TextMeshProUGUI selectedText;

    public delegate void AllTextHasBeenPrinted();
    public static event AllTextHasBeenPrinted OnAllTextHasBeenPrinted;
    public delegate void EachTextHasBeenPrinted();
    public static event EachTextHasBeenPrinted OnEachTextHasBeenPrinted;
    public delegate void FirstTextHasBeenPrinted();
    public static event FirstTextHasBeenPrinted OnFirstTextHasBeenPrinted;

    bool isReachedEndOfTexts;

    private void Awake()
    {
        //Init, all visible characters need to be set to 0
        ResetAllTextMaxVisibleChars();

    }

    private void Start()
    {
        //isPrintingDone = true;
        //isTextLeft = true;
        ////Print the first block of text when scene starts
        PrintNextTextAndInvokeAllHasBeenPrintedIfNot();
        Debug.Log("TextIndex: " + textIndex); 
        ////set default time interavls between printing each char
        //timeBetweenCharPrint = defaultTimeBetweenCharPrint;
    }
    private void Update()
    {

        if (InputReceiver.CheckIf_Use_Pressed() && isPrintingDone && !isReachedEndOfTexts)
        {
            PrintNextTextAndInvokeAllHasBeenPrintedIfNot();
        
        }
       
        else if (InputReceiver.CheckIf_Quit_Pressed())
        {
            isTextLeft = false;
            OnAllTextHasBeenPrinted.Invoke();
        }

    }

    void ShowCurrentlySelectedTextViaAlpha()
    {
        Debug.Log("ShowTextCalled");
        selectedText.alpha = 1;
    }

    void HideCurrentlySelectedTextViaAlpha()
    {
        selectedText.alpha = 0;
    }

    void PrintNextTextAndInvokeAllHasBeenPrintedIfNot()
    {
        //Only call again if printing has ended
        // if (isPrintingDone == true)
        if (CheckIfThereIsTextLeft())
        {
            StartCoroutine(PrintTextAndSelectNextTextWhenDoneRoutine());
            Debug.Log("Reached PrintText Routine Start");

        }
        else
        {
            OnAllTextHasBeenPrinted.Invoke();
            isReachedEndOfTexts = true;
            ResetAllTextMaxVisibleChars();
        }
    }


    void HideAllTextViaAlpha()
    {
        foreach (TextMeshProUGUI textInList in textListToDisplay)
        {
            textInList.alpha = 0;
        }

    }

    bool CheckIfThereIsTextLeft()
    {
        if (textIndex < textListToDisplay.Count)
        {
            //Debug.Log("TextIndex : " + textIndex + "TextCount" + textListToDisplay.Count);
            return true;
        }
        else
        {
            return false;
        }
    }

    void InvokeFirstTextHasBeenPrinted()
    {
        Debug.Log("INVOKEFIRSTTEXTHASbEENPRINTED");
        OnFirstTextHasBeenPrinted.Invoke();
    }

    void SelectNextTextInList()
    {
        selectedText = textListToDisplay[textIndex];
        textIndex++;
    }

    void ResetCurrentTextMaxVisibleChar()
    {
        selectedText.maxVisibleCharacters = 0;
    }

    void ResetAllTextMaxVisibleChars()
    {
        foreach (TextMeshProUGUI textInList in textListToDisplay)
        {
            textInList.maxVisibleCharacters = 0;
        }
    }

    IEnumerator PrintTextAndSelectNextTextWhenDoneRoutine()
    {
        //Reset the last randomly selected text
        ResetCurrentTextMaxVisibleChar();
        //Select a new randomly selected text
        SelectNextTextInList();
        //Show the current text's via alpha
        ShowCurrentlySelectedTextViaAlpha();

        int visibleCount = 0;
        //No need to store a local varaible, somehow this wont get filled anyway
        //int totalLength = selectedText.textInfo.characterCount;

        Debug.Log("Character OCunt: " + selectedText.textInfo.characterCount);

        while (visibleCount <= selectedText.textInfo.characterCount)
        {
            isPrintingDone = false;
            selectedText.maxVisibleCharacters = visibleCount;
            visibleCount++;

            yield return new WaitForSeconds(timeBetweenCharPrint); 
        }

        isPrintingDone = true;
        //Invoke TextHasBeenPrinted after first selected text has finished printing
        if (textIndex == 1)
            InvokeFirstTextHasBeenPrinted();

        yield break;
    }


    private void OnTriggerEnter(Collider other)
    {
        SelectNextTextInList();
        ShowCurrentlySelectedTextViaAlpha();
    }

    //Maybe not useful for sequential text but keep these anyway
    private void OnTriggerStay(Collider other)
    {
        playerIsInTrigger = true;

    }

    private void OnTriggerExit(Collider other)
    {
        playerIsInTrigger = false;
        HideAllTextViaAlpha();
        ResetAllTextMaxVisibleChars();
    }
}
