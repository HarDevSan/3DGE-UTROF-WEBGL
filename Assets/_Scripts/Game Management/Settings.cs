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
    public Slider walkSpeedSlider;

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
    [SerializeField]
    float currentWalkSpeed;

    public PlayerStats playerStatSO;

    private void Awake()
    {
        //arbritarily measured "feelGood" vaues
        defaultSpeedX = 1.5f;
        defaultSpeedY = 0.0125f;
        //Init mult by 1, no change
        speedSliderBoth.value = 1;
        speedSliderX.value = 1;
        speedSliderY.value = 1;

        walkSpeedSlider.value = playerStatSO._defaultWalkSpeed;


        //AddListeners
        speedSliderBoth.onValueChanged.AddListener(delegate { UpdateMouseValues(); });
        speedSliderBoth.onValueChanged.AddListener(delegate { UpdateWalkSpeed(); });

    }

    private void UpdateMouseValues()
    {
        {
            mouseSpeedBoth = speedSliderBoth.value;
            mouseSpeedX = speedSliderX.value;
            mouseSpeedY = speedSliderY.value;

            freeLookCam.m_XAxis.m_MaxSpeed = defaultSpeedX * mouseSpeedBoth * mouseSpeedX;
            freeLookCam.m_YAxis.m_MaxSpeed = defaultSpeedY * mouseSpeedBoth * mouseSpeedY;

            currentVCamspeedX = freeLookCam.m_XAxis.m_MaxSpeed;
            currentVCamspeedY = freeLookCam.m_YAxis.m_MaxSpeed;
            
        }
    }

    private void UpdateWalkSpeed()
    {
        currentWalkSpeed = walkSpeedSlider.value;
        playerStatSO._defaultWalkSpeed = currentWalkSpeed;
        playerStatSO._walkSpeed = currentWalkSpeed;
    }
}
