using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxRotator : MonoBehaviour
{

    //public Material skyBoxMat;
    Transform skyBoxTransform;
    public float rotSpeedX;
    public float rotSpeedY;
    public float rotSpeedZ;


    // Use this for initialization
    void Start()
    {
        //skyBox = RenderSettings.skybox;
        skyBoxTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {

        skyBoxTransform.Rotate(new Vector3(rotSpeedX, rotSpeedY, rotSpeedZ));
        //skyBoxMat.SetFloat("_Rotation", Time.time * rotSpeed);
    }
}

