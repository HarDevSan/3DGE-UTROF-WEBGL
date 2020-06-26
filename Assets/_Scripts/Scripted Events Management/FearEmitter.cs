using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class FearEmitter : MonoBehaviour
{

    [Header("LightList")]
    public List<Light> lightList = new List<Light>();

    [Header("AudioSourcetList")]
    public List<AudioSource> audioSourceList = new List<AudioSource>();

    public UnityEvent playerEntersEmitter;

    private void OnTriggerEnter(Collider other)
    {
        playerEntersEmitter.Invoke();
    }

}
