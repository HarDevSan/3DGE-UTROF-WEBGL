using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReadNote : TextEvent_SequentialAndInvestigate
{
    [Header("Note Content Texts List")]
    public List<TextMeshProUGUI> noteTextList;
    public CanvasGroup backGoundImageGrp;
    public Image foreGroundDarkenImage;
    public float blendInBackGroundSpeed;
    public float blendInBackForeGroundSpeed;
    public float backGroundOpacityMax;
    public float foreGroundOpacityMax;

    /*Disable the class that drives printing before
     Otherwise we have duplicated text*/
    public TextEvent_withInvestigatePrompt parentScript;

    bool isReadingNote;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        /*Do not call base.Update in this case, we do not wanttext printed on interaction,
         * but only after all prompts have been printed*/
        if (isReadingNote)
        {
            base.Update();
        }
        //Blend In forgreound to darken the notes graphical representation
        if (textIndex == 2 && InputReceiver.CheckIf_Use_Pressed())
        {
            BlendInForeGroundToDarkenImage();

        }
    }

    void BlendInForeGroundToDarkenImage()
    {
        StartCoroutine(BlendInForeGroundImageRoutine());
    }

    public void DisableParentScript()
    {
        parentScript.enabled = false;
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
        isReadingNote = true;
          
        if (true)
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

    IEnumerator BlendInForeGroundImageRoutine()
    {
        Color tempCol = new Color(0, 0, 0, foreGroundOpacityMax);

        while (tempCol.a <= foreGroundOpacityMax)
        {
            tempCol.a = Mathf.Lerp(foreGroundDarkenImage.color.a, foreGroundOpacityMax, blendInBackForeGroundSpeed * Time.deltaTime);
            foreGroundDarkenImage.color = tempCol;

            yield return null;
        }

        yield return null;
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
