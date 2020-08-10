using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MaxVisibleCharToZero : MonoBehaviour
{
    public TextMeshProUGUI[] textsToResetMaxVisCharToZeroOnStart;
    //public CanvasGroup[] textsToZeroOutAlpha;

    // Start is called before the first frame update
    private void Awake()
    {
        //textsToZeroOutAlpha = FindObjectsOfType<TextMeshProUGUI>();
        foreach (TextMeshProUGUI txt in textsToResetMaxVisCharToZeroOnStart)
        {
            txt.maxVisibleCharacters = 0;
        }
        //foreach (CanvasGroup txt in textsToZeroOutAlpha)
        //{
        //    txt.alpha = 0;
        //}
    }
}
