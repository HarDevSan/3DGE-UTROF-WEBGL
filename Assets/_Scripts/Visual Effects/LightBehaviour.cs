﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBehaviour : MonoBehaviour
{
    public List<Light> lightsToFLickerlist = new List<Light>();

    [Header("Light Behaviour Toggles")]
    public bool isFlickerPingPong;
    public bool isPhaseLightBySine;
    public bool isHueshiftBySine;

    [Header("Flicker Params")]
    public float flickerSpeed;
    public float flickerLightIntensity;

    [Header("Phasing Params")]
    public float phaseLength;
    public float maxPhasingIntensity;

    [Header("HueShiftParamsRGB")]
    public float redShiftIntensity;
    public float redShiftlength;
    public float blueShiftIntensity;
    public float blueShiftlength;
    public float greenShiftIntensity;
    public float greenShiftlength;

    bool isFlickerRandom;

    public void Update()
    {

        if (isFlickerPingPong)
        {
            FlickerLightPingPong();
        }
        else if (isPhaseLightBySine)
        {
            PhaseLightBySineWave();
        }
        else if (isHueshiftBySine)
        {
            HueShiftBySineWave();
        }

    }

    public void ToggleIsLFickerPingPong()
    {
        isFlickerPingPong = !isFlickerPingPong;
    }

    public void ToggleisPhaseLightBySine()
    {
        isPhaseLightBySine = !isPhaseLightBySine;
    }

    public void ToggleHueShiftBySineWave()
    {
        isPhaseLightBySine = !isPhaseLightBySine;
    }

    public void FlickerLightPingPong()
    {
        foreach (Light light in lightsToFLickerlist)
        {
            {
                Debug.Log("Light should flicker");
                float tempIntensity= Mathf.PingPong(Time.time * flickerSpeed, Random.Range(0,flickerLightIntensity));
                light.intensity = tempIntensity;
            }
        }
    }

    public void PhaseLightBySineWave()
    {
        foreach (Light light in lightsToFLickerlist)
        {
            {
                Debug.Log("Light should Phase");
                light.intensity = Mathf.Sin( maxPhasingIntensity + phaseLength * Time.time);
            }
        }
    }

    public void HueShiftBySineWave()
    {
        foreach (Light light in lightsToFLickerlist)
        {
            {
                Debug.Log("Light should HueShift");

                Color newColor = new Color();

                newColor.r = Mathf.Sin(redShiftIntensity + redShiftlength * Time.time);
                newColor.g = Mathf.Sin(greenShiftIntensity + greenShiftlength * Time.time);
                newColor.b = Mathf.Sin(blueShiftIntensity + blueShiftlength * Time.time);

                light.color = newColor;
            }
        }
    }

    //IEnumerator FlickerPingPongRoutine()
    //{
    //    while (isFlickerPingPong)
    //    {
    //        foreach (Light light in lightsToFLickerlist)
    //        {
    //            light.intensity = Mathf.PingPong(light.intensity, 1f);
    //        }
    //        yield return null;

    //    }
    //    yield break;
    //}

}
