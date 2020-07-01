using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEvent : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textToDisplay;

    [SerializeField]
    bool playerIsInTrigger;
    bool isUserInvestigating;

    private void Awake()
    {
        //PlayerController.OnPlayerSeesSomethingInteractable_Item += ShowText;
        //PlayerController.OnPlayerDoesNotSeeSomehtingInteractable += HideText;
    }

    private void Start()
    {
        textToDisplay.maxVisibleCharacters = 0;
    }

    void ShowText()
    {
        Debug.Log("ShowTextCalled");
        textToDisplay.alpha = 1;
    }

    void PrintText()
    {
        StartCoroutine(PrintTextRoutine());
    }


    void HideText()
    {
        textToDisplay.alpha = 0;

    }

    /* While the text gets printed and the player did not interact a second time,
     * time should be paused so that no unfair occurences happen like an enemy stabbing the player or 
     * player loses valuable total playtime for his/her score */
    void PauseTimeScale()
    {
        Time.timeScale = 0f;
    }

    void ResumeTimeScale()
    {
        Time.timeScale = 1f;
    }

    IEnumerator PrintTextRoutine()
    {
        int totalVisibleCharacters = textToDisplay.textInfo.characterCount;

        int visibleCount = 0;

        while (visibleCount <= totalVisibleCharacters)
        {
            visibleCount++;
            Debug.Log("visible count :" + visibleCount);
            textToDisplay.maxVisibleCharacters = visibleCount;
            yield return new WaitForSeconds(0.066f);
        }

    }



    private void OnTriggerStay(Collider other)
    {
        playerIsInTrigger = true;

        if (InputReceiver.CheckIf_Use_Pressed())
        {
            //isUserInvestigating = !isUserInvestigating;

            //if (isUserInvestigating == true)
            //{
            //    PauseTimeScale();
                PrintText();

            //    }
            //    else
            //    {
            //        ResumeTimeScale();
            //    }

            //}
            //else
            //{

        }




    }

    private void OnTriggerExit(Collider other)
    {
        playerIsInTrigger = false;
        HideText();

    }
}
