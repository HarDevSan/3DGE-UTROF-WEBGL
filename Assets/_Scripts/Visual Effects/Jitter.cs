using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jitter : MonoBehaviour
{
    public Light light;
    [Range(1,3)]
    public float lightRandomangeMin;
    [Range(2, 4)]
    public float lightRandomangeMax; 

    // Update is called once per frame
    void Update()
    {
        light.intensity = Random.Range(lightRandomangeMin, lightRandomangeMax);
    }
}
