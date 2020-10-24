using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyResetter : MonoBehaviour
{
    public Transform sky;
    Renderer rendeRerer;
    Color originalColor;

    public int timeToWait;

    private void Awake()
    {
    }

    private void Start()
    {
        rendeRerer = sky.GetComponent<Renderer>();
    }

 



}
