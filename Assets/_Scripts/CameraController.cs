using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

     CinemachineFreeLook vFreeLookCam;

    [Range(.5f, 1)]
    public float clampYFactorPositve;
    [Range(0f,.5f)]
    public float clampYFactorNegative;




    // Start is called before the first frame update
    void Start()
    {
        vFreeLookCam = GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {

        float axis = vFreeLookCam.m_YAxis.Value;
        if(axis > clampYFactorPositve)
        {
            float lastpos = vFreeLookCam.m_YAxis.Value;

            axis = Mathf.Max(axis, clampYFactorPositve);

        }
        else if(axis < clampYFactorNegative)
        {
            float lastpos = vFreeLookCam.m_YAxis.Value;

            axis = Mathf.Max(axis, clampYFactorNegative);

        }
        vFreeLookCam.m_YAxis.Value = axis;

    }
}
