using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public List<Light> lightsToFLickerlist = new  List<Light>();


    //Toggles
    public bool isFlickerPingPong;

    
    public void FlickerPingPong()
    {
        StartCoroutine(FlickerPingPongRoutine());
    }

    IEnumerator FlickerPingPongRoutine()
    {
        while (isFlickerPingPong)
        {
            foreach (Light light in lightsToFLickerlist)
            {
                light.intensity = Mathf.PingPong(light.intensity, 1f);
            }
            yield return null;

        }
        yield break;
    }

}
