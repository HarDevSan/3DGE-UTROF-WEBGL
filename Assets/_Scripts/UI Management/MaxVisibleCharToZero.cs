using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*This class carries a list of txts. The list of text have their maximum visible chars reset when the game starts. 
 * It is filled in the inspector manually per hand. the alternative to get all txt via GetComponent in Awake or Start
 * and zero out their maximum length is not feasible, as it also zeroes out any txt which should be visible at all times,
 * such as pause menu, or button content*/
public class MaxVisibleCharToZero : MonoBehaviour
{
    public TextMeshProUGUI[] textsToResetMaxVisCharToZeroOnStart;

    private void Awake()
    {
        foreach (TextMeshProUGUI txt in textsToResetMaxVisCharToZeroOnStart)
        {
            txt.maxVisibleCharacters = 0;
        }
        //StartCoroutine(textsToResetMaxVisCharToZeroOnStartRoutine());

    }

    //IEnumerator textsToResetMaxVisCharToZeroOnStartRoutine()
    //{
    //    int i = 0;

    //    while(i< textsToResetMaxVisCharToZeroOnStart.Length)
    //    {
    //        textsToResetMaxVisCharToZeroOnStart[i].maxVisibleCharacters = 0;
    //        i++;
    //        yield return null;
    //    }

    //}
}
