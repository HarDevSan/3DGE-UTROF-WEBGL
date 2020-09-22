using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLightOn : MonoBehaviour
{
    public Light light;
    float defaultLightIntensity;
    public float lightIntensityIncreaseSpeed;
    public LightBehaviour lightBehaviour;
    public float maxFlickerSpeed;
    public float flickerIncreaseSpeed;
    public AudioSource lightGetsTurnedOnSound;
    public GameObject optionalDisappearingObject;


    private void Start()
    {
        defaultLightIntensity = GetComponent<Light>().intensity;
        //Setthe light intensity whie flickering to the lights intensity in the scene
        lightBehaviour.flickerLightIntensity = light.intensity;
    }

    public void StartTurnOnLight()
    {
        StartCoroutine(TurnOnLightRoutine());
    }
    public void StartTurnFLickerBackOn()
    {
        StartCoroutine(TurnFlickerBackOn());
    }

    IEnumerator TurnFlickerBackOn()
    {
        lightBehaviour.isFlickerPingPong = true;
        lightGetsTurnedOnSound.Play();

        float t = 0f;
        float toLerpFrom = 0f;
        float toLerpTo = maxFlickerSpeed;
        float lerpValue = 0f;

        while (lerpValue < toLerpTo)
        {
            //Increase flicker
            t += flickerIncreaseSpeed * Time.deltaTime;
            lerpValue = Mathf.Lerp(toLerpFrom, toLerpTo, t);
            lightBehaviour.flickerSpeed = lerpValue;
            yield return null;
        }

        lightBehaviour.isFlickerPingPong = false;

    }

    IEnumerator TurnOnLightRoutine()
    {
        optionalDisappearingObject.SetActive(false);

        lightBehaviour.isFlickerPingPong = true;
        lightGetsTurnedOnSound.Play();

        float t = 0f;
        float toLerpFrom = 0f;
        float toLerpTo = defaultLightIntensity;
        float lerpValue = 0f;

        while (light.intensity < defaultLightIntensity)
        {
            //Increase light intensity
            t += lightIntensityIncreaseSpeed * Time.deltaTime;
            lerpValue = Mathf.Lerp(toLerpFrom, toLerpTo, t);
            light.intensity = lerpValue;
            yield return null;
        }
        lightBehaviour.isFlickerPingPong = false;
    }
}
