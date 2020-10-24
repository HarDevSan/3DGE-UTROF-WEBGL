using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Collections;


public class LutSwitcher : MonoBehaviour
{

    public Volume postFXVolume;

    ColorLookup colorLookUp;


    public float blendInSpeed;

    private void Start()
    {
        postFXVolume.profile.TryGet(out colorLookUp);

    }

    public void BlendLUTByWeight()
    {
        StartCoroutine(BlendInWeightRoutine());
        StartCoroutine(BlendInLUTRoutine());
    }

    IEnumerator BlendInWeightRoutine()
    {
        float from = 0f;
        float to = 1f;
        float t = 0f;
        float value = 0f;

        while (value < 1)
        {

            t += blendInSpeed * Time.deltaTime;
            value = Mathf.Lerp(from, to, t);
            postFXVolume.weight = value;
            yield return null;
        }
    }

    IEnumerator BlendInLUTRoutine()
    {

        float from = 0f;
        float to = 1f;
        float t = 0f;
        float value = 0f;

        while (value < 1f)
        {
            t += blendInSpeed * Time.deltaTime;
            value = Mathf.Lerp(from, to, t);
            colorLookUp.contribution.value = value;

            yield return null;
        }

    }
}
