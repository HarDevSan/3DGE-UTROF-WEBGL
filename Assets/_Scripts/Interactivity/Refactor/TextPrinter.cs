using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPrinter : MonoBehaviour
{
    /* List of texts representing fragments or sentences that will be displaed on player 
    * investigation, can be randoml selected from. Should be tied to the item/prop this 
    * TextEvent script is attached to*/
    public List<TextMeshProUGUI> TextListToDisplay { get; set; }
    private List<TextMeshProUGUI> textToDisplayList;

    //Time between each char that gets printed on the Canvas
    [Header("PrintTime")]
    public float defaultTimeBetweenCharPrint;
    public float timeBetweenCharPrint;
    public float timeBetweenCharPrintWhenPlayerPressedUse;

    public int textIndex;


    [Header("Bools")]
    [SerializeField]
    bool playerIsInTrigger;
    [SerializeField]
    bool isUserInvestigating;
    [SerializeField]
    bool isPrintingDone;
    [SerializeField]
    bool isTextLeft;

    [SerializeField]
    int pressedUseCount;

    bool isReachedEndOfTexts;


    //global var holding a random textObject
    [Header("Currentl selected text")]
    [SerializeField]
    TextMeshProUGUI selectedText;

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
        if (CheckIfThereIsTextLeft())
            StartCoroutine(PrintTextAndSelectNextTextWhenDoneRoutine());
        else
        {
            isReachedEndOfTexts = true;
            ResetAllTextMaxVisibleChars();
        }
    }


    void HideAllTextViaAlpha()
    {
        foreach (TextMeshProUGUI textInList in TextListToDisplay)
        {
            textInList.alpha = 0;
        }

    }

    bool CheckIfThereIsTextLeft()
    {
        if (textIndex < TextListToDisplay.Count)
        {
            return true;
        }
        else
        {

            return false;

        }
    }
    void SelectNextTextInList()
    {

        selectedText = TextListToDisplay[textIndex];
        textIndex++;


    }

    void ResetCurrentTextMaxVisibleChar()
    {
        selectedText.maxVisibleCharacters = 0;
    }


    void ResetAllTextMaxVisibleChars()
    {
        foreach (TextMeshProUGUI textInList in TextListToDisplay)
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

        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        SelectNextTextInList();
        ShowCurrentlySelectedTextViaAlpha();
    }



    private void OnTriggerExit(Collider other)
    {
        playerIsInTrigger = false;
        HideAllTextViaAlpha();
        ResetAllTextMaxVisibleChars();
    }
}
