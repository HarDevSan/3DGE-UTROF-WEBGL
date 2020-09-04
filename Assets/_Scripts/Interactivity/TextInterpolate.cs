using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextInterpolate : MonoBehaviour
{
    public TextMeshProUGUI textToBlendIn;
    public float blendInAmount;

    void Awake()
    {
        TextEvent_Sequential.OnFirstTextHasBeenPrinted += BLendInText;
    }

    public void BLendInText()
    {
        StartCoroutine(BlendInTextRoutine());
    }

    IEnumerator BlendInTextRoutine()
    {
        while (textToBlendIn.alpha < 1)
        {

            textToBlendIn.alpha = Mathf.Lerp(textToBlendIn.alpha, 1, blendInAmount * Time.deltaTime);

            yield return null;
        }

    }
}
