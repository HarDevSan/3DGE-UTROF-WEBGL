using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;


public class Settings : MonoBehaviour
{
    public CinemachineFreeLook freeLookCam;
    public Slider speedSliderBoth;
    public Slider speedSliderX;
    public Slider speedSliderY;

    float defaultSpeedX;
    float defaultSpeedY;

    [SerializeField]
    float mouseSpeedX;
    [SerializeField]
    float mouseSpeedY;
    [SerializeField]
    float mouseSpeedBoth;
    [SerializeField]
    float currentVCamspeedX;
    [SerializeField]
    float currentVCamspeedY;

    private void Awake()
    {
        defaultSpeedX = 3f;
        defaultSpeedY = 0.025f;
        speedSliderBoth.value = 1;
        speedSliderX.value = 1;
        speedSliderY.value = 1;

    }

    private void Update()
    {
        {
            mouseSpeedBoth = speedSliderBoth.value;
            mouseSpeedX = speedSliderX.value;
            mouseSpeedY = speedSliderY.value;

            freeLookCam.m_XAxis.m_MaxSpeed = defaultSpeedX * mouseSpeedBoth * mouseSpeedX;
            freeLookCam.m_YAxis.m_MaxSpeed = defaultSpeedY * mouseSpeedBoth * mouseSpeedY; ;

            //currentVCamspeedX = freeLookCam.m_XAxis.m_MaxSpeed;
            //currentVCamspeedY = freeLookCam.m_YAxis.m_MaxSpeed;

        }
    }
}
