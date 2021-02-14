using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TAudioEventRandomPitch : MonoBehaviour
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

    private void Update()
    {
        if (playerIsInTrigger && InputReceiver.CheckIf_Use_Pressed() && PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_item)
        {
           

        }
        else if (playerIsInTrigger && PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_item == false)
        {
            Debug.Log("Player is not looking at the object");
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
