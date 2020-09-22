using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakLight : MonoBehaviour
{
    public Light light;
    public LightBehaviour lightBehaviour;
    public float maxFlickerSpeed;
    public float flickerIncreaseSpeed;
    public AudioSource lightBreakingSound;

    public GameObject optionalDisappearingObject;
    public bool isLetObjectDisappear;

    private void Start()
    {
        //Setthe light intensity whie flickering to the lights intensity in the scene
        lightBehaviour.flickerLightIntensity = light.intensity;
    }

    public void StartBreakLight()
    {
        StartCoroutine(BreakLightsRoutine());
    }

    IEnumerator BreakLightsRoutine()
    {
        lightBehaviour.isFlickerPingPong = true;
        float t = 0f;
        float toLerpFrom = 0f;
        float toLerpTo = maxFlickerSpeed;
        float lerpValue = 0f;

        while (lerpValue < toLerpTo)
        {
            t += flickerIncreaseSpeed * Time.deltaTime;
            lerpValue = Mathf.Lerp(toLerpFrom, toLerpTo, t);
            lightBehaviour.flickerSpeed = lerpValue;
            yield return null;
        }
        lightBehaviour.isFlickerPingPong = false;
        lightBreakingSound.Play();
        light.intensity = 0f;

        if (isLetObjectDisappear)
        {
            optionalDisappearingObject.SetActive(false);
            GameObject.Destroy(optionalDisappearingObject);
        }
    }

   


}
