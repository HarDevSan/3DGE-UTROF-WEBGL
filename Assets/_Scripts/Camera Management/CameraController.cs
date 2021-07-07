using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    public CinemachineFreeLook vFreeLookCam;

    CinemachineVirtualCameraBase vCamBase_MiddleRig;

    public CinemachineCollider cinemachineCollider;

    public CinemachineBrain brain;

    string defaultAxisNameX;
    string defaultAxisNameY;

    bool isCollsionGoingOn;
    public Transform defaultLookAt;
    public Transform alternateLookAt;

    private void Start()
    {
        defaultAxisNameX = vFreeLookCam.m_XAxis.m_InputAxisName;
        defaultAxisNameY = vFreeLookCam.m_YAxis.m_InputAxisName;

        //cache rig
        vCamBase_MiddleRig = vFreeLookCam.GetRig(1);

    }

    void LateUpdate()
    {
        /*When brain is blending from cam to another, we dont want any axis input, 
        *therefore we set the axis temporarily to empty strings*/
        if (brain.IsBlending)
        {
            vFreeLookCam.m_XAxis.m_InputAxisName = "";
            vFreeLookCam.m_YAxis.m_InputAxisName = "";
        }

        //Check for Camera Displacement
        SwapTargetOnCollision();
    }

    void SwapTargetOnCollision()
    {
        isCollsionGoingOn = cinemachineCollider.CameraWasDisplaced(vCamBase_MiddleRig);

        if (isCollsionGoingOn == true)
        {
            vFreeLookCam.LookAt = alternateLookAt;
        }
        else if (isCollsionGoingOn == false)
        {
            vFreeLookCam.LookAt = defaultLookAt;
        }
        else
        {
            vFreeLookCam.LookAt = defaultLookAt;
        }
    }

    /*Acces this via Signal in timeline, so we prevent polluting Update with 
     * further instructions per frame and only reset the axis once*/
    public void ResetAxisNamesAfterBlend()
    {
        vFreeLookCam.m_XAxis.m_InputAxisName = defaultAxisNameX;
        vFreeLookCam.m_YAxis.m_InputAxisName = defaultAxisNameY;
    }


}
