using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyColorChanger : MonoBehaviour
{
    public Transform sky;
    public Transform moon;
    Renderer rendeRererSky;
    Renderer rendeRererMoon;

    public Color colortochangeTo;

    public float lerpSpeed;
    Color originalColorSky;
    Color originalColorMoon;

    public int timeToWait;

    bool isAlreadyTriggered;

    private void Start()
    {
        rendeRererSky = sky.GetComponent<Renderer>();
        rendeRererMoon = moon.GetComponent<Renderer>();
        originalColorSky = rendeRererSky.material.GetColor("_BaseColor");
        originalColorMoon = rendeRererMoon.material.GetColor("_BaseColor");

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
    public void StartResetColor()
    {
        StartCoroutine(SetSkyToOriginalColorRoutine());
    }

    IEnumerator SetSkyToOriginalColorRoutine()
    {

        yield return new WaitForSeconds(timeToWait);

        var block = new MaterialPropertyBlock();

        block.SetColor("_BaseColor", originalColorSky);

        rendeRererSky.SetPropertyBlock(block);
        rendeRererMoon.SetPropertyBlock(block);


    }

    IEnumerator InterpolSkyColorRoutine()
    {

        var block = new MaterialPropertyBlock();

        float t = 0f;

        Color fromColor = new Color(0, 0, 0, 1);
        Color tempCol = rendeRererSky.material.color;

        while (t < 1)
        {
            t += Time.deltaTime * lerpSpeed;

            tempCol = Color.Lerp(fromColor, colortochangeTo, t);

            block.SetColor("_BaseColor", tempCol);

            rendeRererSky.SetPropertyBlock(block);
            rendeRererMoon.SetPropertyBlock(block);

            yield return null;
        }
        Debug.Log("Reached end of ColInterp rtine");
    }
}

