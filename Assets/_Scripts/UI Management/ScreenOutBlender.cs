using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOutBlender : MonoBehaviour
{
    public float blendOutSpeed;

    public CanvasGroup groupToBlendOut;

    float perc;

    public delegate void SreenFinsihedBeingBlendedOut();
    public static event SreenFinsihedBeingBlendedOut OnScreenFinsihedBeingBlendedOut;

    private void Awake()
    {
        TextEvent_Sequential.OnAllTextHasBeenPrinted += BlendOutGroup;
    }

    void BlendOutGroup()
    {
        StartCoroutine(BlendOutGroupRoutine());
    }

    IEnumerator BlendOutGroupRoutine()
    {


        while (groupToBlendOut.alpha > 0.005)
        {
            groupToBlendOut.alpha = Mathf.Lerp(groupToBlendOut.alpha, 0, blendOutSpeed * Time.deltaTime);
         
            yield return null;
        }

        Debug.Log("Reached BlendOut");
        OnScreenFinsihedBeingBlendedOut.Invoke();

    }

}
