using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOutBlender : MonoBehaviour
{
    public float blendOutSpeed;

    public AudioSource introAudioSource;

    private void Awake()
    {
        TextEvent_Sequential.OnAllTextHasBeenPrinted += BlendOutGroup;
    }

    void BlendOutGroup()
    {
        StartCoroutine(BlendOutGroupRoutine());
    }

    IEnumerator BlendOutGroupRoutine()
    {

        while (introAudioSource.volume > 0.005)
        {
            introAudioSource.volume = Mathf.Lerp(introAudioSource.volume, 0, blendOutSpeed * Time.deltaTime );
         
            yield return null;
        }

        Debug.Log("Reached BlendOut");

    }

}
