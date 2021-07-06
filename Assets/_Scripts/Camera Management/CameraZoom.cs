using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    public CinemachineFreeLook zoomCam;
    public CinemachineVirtualCamera zoomCamFPS;


    private void Awake()
    {
        InputReceiver.On_Z_Input += RaiseZoomCamPrio;
        InputReceiver.On_Z_Input_Up += LowerZoomCamPrio;
    }

    void RaiseZoomCamPrio()
    {
        zoomCamFPS.Priority = zoomCamFPS.m_Priority +1;
    }
    void LowerZoomCamPrio()
    {
        zoomCamFPS.Priority = zoomCamFPS.m_Priority -1;
    }
}
