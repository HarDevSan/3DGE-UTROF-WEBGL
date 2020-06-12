using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxRotator : MonoBehaviour
{

    public Material skyBoxMat;
    public float rotSpeed;

    // Use this for initialization
    void Start()
    {
        //skyBox = RenderSettings.skybox;
    }

    // Update is called once per frame
    void Update()
    {


        skyBoxMat.SetFloat("_Rotation", Time.time * rotSpeed);
    }
}

