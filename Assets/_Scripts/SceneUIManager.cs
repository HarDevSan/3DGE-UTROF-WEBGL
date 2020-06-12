using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneUIManager : MonoBehaviour
{
    public GameObject text;
    public CanvasGroup canvasGroup;





    private void OnTriggerEnter(Collider other)
    {
        canvasGroup.alpha = 1;
    }

    private void OnTriggerExit(Collider other)
    {
        canvasGroup.alpha = 0;
    }




}

