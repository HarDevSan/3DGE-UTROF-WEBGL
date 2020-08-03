using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextEvent_Sequential : MonoBehaviour
{
    /* List of texts representing fragments or sentences that will be displaed on player 
     * investigation, can be randoml selected from. Should be tied to the item/prop this 
     * TextEvent script is attached to*/
    [Header("Texts List")]
    public List<TextMeshProUGUI> textListToDisplay;

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

    //global var holding a random textObject
    [Header("Currentl selected text")]
    [SerializeField]
    TextMeshProUGUI selectedText;

    public delegate void AllTextHasBeenPrinted();
    public static event AllTextHasBeenPrinted OnAllTextHasBeenPrinted;
    public delegate void EachTextHasBeenPrinted();
    public static event EachTextHasBeenPrinted OnEachTextHasBeenPrinted;
    public delegate void FirstTextHasBeenPrinted();
    public static event FirstTextHasBeenPrinted OnFirstTextHasBeenPrinted;

    private void Awake()
    {
        //Init
        ResetAllTextMaxVisibleChars();

    }

    private void Start()
    {

        isPrintingDone = true;
        isTextLeft = true;
        ////Print the first block of text when scene starts
        PrintNextTextAndInvokeAllHasBeenPrintedIfNot();

        ////set default time interavls between printing each char
        //timeBetweenCharPrint = defaultTimeBetweenCharPrint;
    }

    private void Update()
    {
        CheckIfThereIsTextLeft();

        Debug.Log(isTextLeft);
        if (InputReceiver.CheckIf_Use_Pressed() && isPrintingDone)
        {
            PrintNextTextAndInvokeAllHasBeenPrintedIfNot();
            //Should eable dynamic text blend speed, postponed
            //pressedUseCount++;
        }
        else
        {
            //pressedUseCount = 0;

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
        Debug.Log("Reached PrintText");
        //Only call again if printing has ended
        // if (isPrintingDone == true)
        if (isTextLeft)
            StartCoroutine(PrintTextAndSelectNextTextWhenDoneRoutine());
        else
        {
            OnAllTextHasBeenPrinted.Invoke();
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

    void CheckIfThereIsTextLeft()
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

    /* While the text gets printed and the player did not interact a second time,
     * time should be paused so that no unfair occurences happen like an enemy stabbing the player or 
     * the player losing valuable total playtime for his/her score */
    void PauseTimeScale()
    {
        Time.timeScale = 0f;
    }

    void ResumeTimeScale()
    {
        Time.timeScale = 1f;
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

        //Reet the last randomly selected text
        ResetCurrentTextMaxVisibleChar();
        //Select a new randomly selected text
        SelectNextTextInList();

        //Show the current text's via alpha
        ShowCurrentlySelectedTextViaAlpha();

        int visibleCount = 0;
        int totalLength = selectedText.textInfo.characterCount;

        //Debug.Log("CharCount total: " + totalLength);

        while (visibleCount <= totalLength)
        {
            isPrintingDone = false;
            Debug.Log("Reached While Loop");
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
        //Invoke TextHasBeenPrinted after first selected text has finished printing
        if (textIndex == 1)
            InvokeFirstTextHasBeenPrinted();
        //OnEachTextHasBeenPrinted.Invoke();

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
