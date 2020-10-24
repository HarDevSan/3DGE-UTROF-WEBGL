using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class DrunkenSinus : MonoBehaviour
{

    public Volume postFXVolume;

    ChromaticAberration chromaticAberration;
    LensDistortion lensDistortion;

    public float pingPongTimer;
    public float pingPongDecrementInterval;
    public float chromaRange;
    public float lensDistRange;

    bool isPingPong;

    [SerializeField]
    float defaultLensDistoritonValue;
    [SerializeField]
    float defaultLensChromaValue;

    float valueLensDist;
    float valueChroma;
    private void Start()
    {
        postFXVolume.profile.TryGet(out lensDistortion);

        defaultLensDistoritonValue = lensDistortion.intensity.value;

    }

    public void StartPingPong()
    {
        //isPingPong= true;
        StartCoroutine(LensDistortionPingPong());
    }

    //private void Update()
    //{

    //    if (isPingPong)
    //    {
    //        valueLensDist = Mathf.PingPong(Time.time, lensDistRange);
    //        valueChroma = Mathf.PingPong(Time.time, chromaRange);
    //        lensDistortion.intensity.value = valueLensDist;

    //        if (pingPongTimer > 0)
    //        {
    //            pingPongTimer -= pingPongDecrementInterval;
    //        }
    //        else
    //        {
    //            isPingPong = false;
    //            valueLensDist = 0f;
    //            valueChroma = 0f;
    //            lensDistortion.intensity.value = defaultLensDistoritonValue;
    //        }

    //    }

    IEnumerator LensDistortionPingPong()
    {

        while (pingPongTimer > 0)
        {
            valueLensDist = Mathf.PingPong(Time.time, lensDistRange);
            lensDistortion.intensity.value = valueLensDist;
            pingPongTimer -= pingPongDecrementInterval;
            yield return null;
        }
        isPingPong = false;
        lensDistortion.intensity.value = defaultLensDistoritonValue;

    }

}

