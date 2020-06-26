using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public List<Light> lightsToFLickerlist = new List<Light>();

    public List<float>


    //Toggles
    public bool isFlickerPingPong;

    private void Start()
    {
        fo
    }

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
                light.intensity = Mathf.PingPong(0, 1f);
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
