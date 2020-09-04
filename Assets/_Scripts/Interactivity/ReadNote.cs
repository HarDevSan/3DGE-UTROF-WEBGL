using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadNote : TextEvent_SequentialAndInvestigate
{
    [Header("Note Content Texts List")]
    public List<TextMeshProUGUI> noteTextList;
    public CanvasGroup backGoundImageGrp;
    public float blendInBackGroundSpeed;
    public float backGroundOpacityMax;

    public override void Start()
    {
        base.Start();
    }


    public override void SelectNextTextInList()
    {
        if (textIndex < noteTextList.Count && textIndex >= 0)
        {
            selectedText = noteTextList[textIndex];
            textIndex++;
        }
    }



    public override void CheckIfThereIsTextLeft()
    {
        if (textIndex < noteTextList.Count)
        {
            isTextLeft = true;
        }
        else
        {

            isTextLeft = false;

        }
    }

    public override void ResetAllTextMaxVisibleChars()
    {
        foreach (TextMeshProUGUI textInList in noteTextList)
        {
            textInList.maxVisibleCharacters = 0;
        }
    }

    public void StartPrintingNoteContent()
    {
        Debug.Log("is textLeft: " + isTextLeft);
        ////Pause Time while printing            
        //PauseTimeScale();
        if (isTextLeft)
        {
            StartCoroutine(PrintNoteTextRoutine());
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


    IEnumerator BlendInBackGroundImageRoutine()
    {
        while (backGoundImageGrp.alpha < backGroundOpacityMax - .001f)
        {
            backGoundImageGrp.alpha = Mathf.Lerp(backGoundImageGrp.alpha, backGroundOpacityMax, blendInBackGroundSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator BlendOutBackGroundImageRoutine()
    {
        while (backGoundImageGrp.alpha > 0.001)
        {
            backGoundImageGrp.alpha = Mathf.Lerp(backGoundImageGrp.alpha, 0, blendInBackGroundSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator PrintNoteTextRoutine()
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
            // Debug.Log("Visible count : " + visibleCount + " VS total of" + selectedText.maxVisibleCharacters);
            selectedText.maxVisibleCharacters = visibleCount;
            visibleCount++;

            yield return new WaitForSeconds(timeBetweenCharPrint);

        }

        isPrintingDone = true;

        yield break;
    }
}
