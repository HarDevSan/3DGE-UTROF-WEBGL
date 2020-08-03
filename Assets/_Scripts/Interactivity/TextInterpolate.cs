using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextInterpolate : MonoBehaviour
{
    public TextMeshProUGUI textToBlendIn;
    public float blendInAmount;

    public bool shouldBlendIn;

    // Start is called before the first frame update
    void Awake()
    {
        TextEvent_Sequential.OnFirstTextHasBeenPrinted += BLendInText;
    }



    public void BLendInText()
    {
        if (shouldBlendIn)
        {
            StartCoroutine(BlendInTextRoutine());

        }
    }
    
    IEnumerator BlendInTextRoutine()
    {
        while(textToBlendIn.alpha < 1)
        {
            textToBlendIn.alpha = Mathf.Lerp(textToBlendIn.alpha , 1,  blendInAmount * Time.time);

            yield return null;
        }

    }
}
