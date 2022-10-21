using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Range(1, 5)]
    public float lightBaseIntensity;
    [Range(2, 100)]
    public float lightRandomIntensityMax;


    public bool isFlickering;

    public Light lightToFlicker;

    float flickerInterval;
    [Range(0, 0.005f)]
    public float minFlickerInterval;
    [Range(0.01f, .5f)]
    public float maxFlickerInterval;


    private void Start()
    {
        StartCoroutine(LightFlickerRoutine());
    }

    void RandomizeLightRange()
    {
        lightToFlicker.intensity = Random.Range(lightBaseIntensity, lightRandomIntensityMax);
    }

    void RandomizeFlickerInterval()
    {
        flickerInterval = Random.Range(minFlickerInterval, maxFlickerInterval);
    }

    IEnumerator LightFlickerRoutine()
    {
        while (isFlickering)
        { 
            RandomizeLightRange();
            RandomizeFlickerInterval();
            yield return new WaitForSeconds(flickerInterval);
        }

        yield return null;
    }
}
