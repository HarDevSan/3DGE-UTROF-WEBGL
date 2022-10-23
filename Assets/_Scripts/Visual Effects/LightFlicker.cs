using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Range(1, 5)]
    public float lightBaseIntensity;
    [Range(2, 100)]
    public float lightRandomIntensityMax;

    public GameObject emissiveObjectToFlicker;

    public bool isFlickering;

    public Light lightToFlicker;

    float flickerInterval;
    
    float emissiveIntensity;
    Color defaultEmissiveColor;


    [Range(0, 0.005f)]
    public float minFlickerInterval;
    [Range(0.01f, .5f)]
    public float maxFlickerInterval;

    [Range(0f, .5f)]
    public float minEmissionIntensity;
    [Range(0.5f, 2.8f)]
    public float maxEmissionIntensity;

    private void Start()
    {
        StartCoroutine(LightFlickerRoutine());
        defaultEmissiveColor = emissiveObjectToFlicker.GetComponent<Renderer>().material.GetColor("_EmissionColor");
    }

    void RandomizeLightRange()
    {
        lightToFlicker.intensity = Random.Range(lightBaseIntensity, lightRandomIntensityMax);
    }

    void RandomizeFlickerInterval()
    {
        flickerInterval = Random.Range(minFlickerInterval, maxFlickerInterval);
    }

    void RanomdizeEmissiveIntensity()
    {
        emissiveIntensity = Random.Range(minEmissionIntensity, maxEmissionIntensity);
    }

    void FlickerEmissiveObject()
    {
        Color emissiveColor = emissiveObjectToFlicker.GetComponent<Renderer>().material.GetColor("_EmissionColor");
        emissiveObjectToFlicker.GetComponent<Renderer>().material.SetColor("_EmissionColor", emissiveColor * emissiveIntensity);
    }

    void ResetEmissiveColor()
    {
        emissiveObjectToFlicker.GetComponent<Renderer>().material.SetColor("_EmissionColor", defaultEmissiveColor);
    }


    IEnumerator LightFlickerRoutine()
    {
        while (isFlickering)
        {
            RandomizeLightRange();
            RandomizeFlickerInterval();
            RanomdizeEmissiveIntensity();
            FlickerEmissiveObject();
            yield return new WaitForSeconds(flickerInterval);
            ResetEmissiveColor();
        }

        yield return null;
    }
}
