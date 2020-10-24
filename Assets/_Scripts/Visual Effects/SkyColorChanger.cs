using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyColorChanger : MonoBehaviour
{
    public Transform sky;
    Renderer rendeRerer;
    public Color colortochangeTo;

    public float lerpSpeed;
    Color originalColor;

    public int timeToWait;

    bool isAlreadyTriggered;

    private void Start()
    {
        rendeRerer = sky.GetComponent<Renderer>();
        originalColor = rendeRerer.material.GetColor("_BaseColor");
    }

    public void ChangeSkyColor()
    {
        StartCoroutine(InterpolSkyColorRoutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAlreadyTriggered == false)
        {
            ChangeSkyColor();
            StartResetColor();
            isAlreadyTriggered = true;
        }

    }
    void StartResetColor()
    {
        StartCoroutine(SetSkyToOriginalColorRoutine());
    }

    IEnumerator SetSkyToOriginalColorRoutine()
    {

        yield return new WaitForSeconds(timeToWait);

        var block = new MaterialPropertyBlock();

        block.SetColor("_BaseColor", originalColor);

        rendeRerer.SetPropertyBlock(block);

    }

    IEnumerator InterpolSkyColorRoutine()
    {

        var block = new MaterialPropertyBlock();

        float t = 0f;

        Color fromColor = new Color(0, 0, 0, 1);
        Color tempCol = rendeRerer.material.color;

        while (t < 1)
        {
            t += Time.deltaTime * lerpSpeed;

            tempCol = Color.Lerp(fromColor, colortochangeTo, t);

            block.SetColor("_BaseColor", tempCol);

            rendeRerer.SetPropertyBlock(block);

            yield return null;
        }
        Debug.Log("Reached end of ColInterp rtine");
    }
}

