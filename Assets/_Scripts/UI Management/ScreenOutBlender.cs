using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenOutBlender : MonoBehaviour
{
    public float blendOutGroupSpeed;
    public float blendOutPromptSayingPressESpeed;

    public CanvasGroup groupToBlendOut;
    public TextMeshProUGUI pressEpromptToBlendOut;

    public delegate void SreenFinsihedBeingBlendedOut();
    public static event SreenFinsihedBeingBlendedOut OnScreenFinsihedBeingBlendedOut;

    private void Awake()
    {
        TextEvent_Sequential.OnAllTextHasBeenPrinted += BlendOutGroup;
        TextEvent_Sequential.OnAllTextHasBeenPrinted += BlendOutPromptSayingPressE;

    }

    void BlendOutGroup()
    {
        StartCoroutine(BlendOutGroupRoutine());

    }

    void BlendOutPromptSayingPressE()
    {
        StartCoroutine(BlendOutpromptSayingPressERoutine());
    }

    IEnumerator BlendOutGroupRoutine()
    {


        while (groupToBlendOut.alpha > 0.01)
        {
            groupToBlendOut.alpha = Mathf.Lerp(groupToBlendOut.alpha, 0, blendOutGroupSpeed * Time.deltaTime);
         
            yield return null;
        }

        Debug.Log("Reached BlendOut");
        OnScreenFinsihedBeingBlendedOut.Invoke();

    }
    IEnumerator BlendOutpromptSayingPressERoutine()
    {


        while (pressEpromptToBlendOut.alpha > 0.01)
        {
            pressEpromptToBlendOut.alpha = Mathf.Lerp(pressEpromptToBlendOut.alpha, 0, blendOutPromptSayingPressESpeed * Time.deltaTime);

            yield return null;
        }

        Debug.Log("Reached BlendOut");
        OnScreenFinsihedBeingBlendedOut.Invoke();

    }

}
