using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public List<Light> lightsToFLickerlist = new List<Light>();

    public float minIntensity;
    public float maxIntensity;

    //Toggles
    public bool isFlickerPingPong;

   

    public void Update()
    {
        if (isFlickerPingPong)
        {
            FlickerLightPingPong();
        }
    }

    public void FlickerLightPingPong()
    {
        foreach (Light light in lightsToFLickerlist)
        {
            {
                light.intensity = Mathf.PingPong(minIntensity, maxIntensity);
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
