using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextEventRandom : MonoBehaviour
{
    /* List of texts representing fragments or sentences that will be displaed on player 
     * investigation, can be randoml selected from. Should be tied to the item/prop this 
     * TextEvent script is attached to*/
    [Header("Texts List")]
    public List<TextMeshProUGUI> textListToDisplay;

    public float interactionPromptBlendTime;
    public CanvasGroup backGround;

    [Header("Bools")]
    [SerializeField]
    bool playerIsInTrigger;
    [SerializeField]
    bool isUserInvestigating;
    [SerializeField]
    bool isPrintingDone;

    //Time between each char that gets printed on the Canvas
    [Header("PrintTime")]
    public float timeBetweenCharPrint;

    //global var holding a random textObject
    [Header("Currentl selected text")]
    [SerializeField]
    TextMeshProUGUI randomlySelectedText;

    int currentRandomInt;

    public delegate void FirstTextHasBeenPrinted();
    public static event FirstTextHasBeenPrinted OnFirstTextHasBeenPrinted;

    private void Awake()
    {
        //Events not fesable here
        //PlayerController.OnPlayerSeesSomethingInteractable_Item += ShowText;
        //PlayerController.OnPlayerDoesNotSeeSomehtingInteractable += HideText;
    }

    private void Start()
    {
        foreach (TextMeshProUGUI textInList in textListToDisplay)
        {
            textInList.maxVisibleCharacters = 0;
        }
        //Init
        isPrintingDone = true;
        currentRandomInt = 0;
        randomlySelectedText = textListToDisplay[currentRandomInt];

    }

    private void Update()
    {
        if (playerIsInTrigger && InputReceiver.CheckIf_Use_Pressed() && PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_item)
        {
  
            PrintText();

        }
        else if (playerIsInTrigger && PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_item == false)
        {
            HideAllTextViaAlpha();
            Debug.Log("Player is not looking at the object");
        }



    }



    void ShowCurrentlySelectedTextViaAlpha()
    {
        Debug.Log("ShowTextCalled");
        randomlySelectedText.alpha = 1;
    }

    void HideCurrentlySelectedTextViaAlpha()
    {
        randomlySelectedText.alpha = 0;

    }

    void PrintText()
    {
        //Only call again if printing has ended
        if (isPrintingDone == true)
            StartCoroutine(PrintTextAndSelectRandomTextWhenDoneRoutine());
    }


    void HideAllTextViaAlpha()
    {
        foreach (TextMeshProUGUI textInList in textListToDisplay)
        {
            textInList.alpha = 0;
        }

    }


    public void FreezePlayerControls()
    {
        InputReceiver.BlockMovementInput();
    }

    public void UnFreezePlayerControls()
    {
        InputReceiver.UnBlockMovementInputs();
    }


    void SelectRandomTextInList()
    {
        int randomTextRange = Random.Range(0, textListToDisplay.Count);

        if (currentRandomInt == randomTextRange)
        {
            while (currentRandomInt == randomTextRange)
                randomTextRange = Random.Range(0, textListToDisplay.Count);
        }
        currentRandomInt = randomTextRange;
        randomlySelectedText = textListToDisplay[randomTextRange];
    }

    void ResetCurrentTextMaxVisibleChar()
    {
        randomlySelectedText.maxVisibleCharacters = 0;
    }

    void ResetAllTextMaxVisibleChars()
    {
        foreach (TextMeshProUGUI textInList in textListToDisplay)
        {
            textInList.maxVisibleCharacters = 0;
        }
    }



    IEnumerator PrintTextAndSelectRandomTextWhenDoneRoutine()
    {
        //Reet the last randomly selected text
        ResetCurrentTextMaxVisibleChar();
        //Select a new randomly selected text
        SelectRandomTextInList();
        //Show the current text's via alpha
        ShowCurrentlySelectedTextViaAlpha();

        int visibleCount = 0;
        int totalLength = randomlySelectedText.textInfo.characterCount;

        Debug.Log("CharCount total: " + totalLength);

        while (visibleCount <= totalLength)
        {
            isPrintingDone = false;

            Debug.Log("Visible count : " + visibleCount + " VS total of" + randomlySelectedText.maxVisibleCharacters);
            randomlySelectedText.maxVisibleCharacters = visibleCount;
            visibleCount++;

            yield return new WaitForSeconds(timeBetweenCharPrint);
        }

        isPrintingDone = true;

        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        SelectRandomTextInList();
        ShowCurrentlySelectedTextViaAlpha();
    }

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
