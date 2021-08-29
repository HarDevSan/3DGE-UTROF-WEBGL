using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraOcclusion : MonoBehaviour
{
    public CinemachineFreeLook occludedCam;
    public CinemachineFreeLook mainCam;
    public CinemachineFreeLook thisCam;


    public CinemachineCollider cinemachineCollider;

    bool isCameraWasDisplaced;

    bool isPriorityChangedOnce;

    private void Update()
    {
        CheckIfCameraWasDisplaced();

        if (isCameraWasDisplaced)
        {
            RaiseOccludedCamPrio();
        }
        else
        {
            LowerOccludedCamPrio();
        }
    }


    void CheckIfCameraWasDisplaced()
    {
        isCameraWasDisplaced = cinemachineCollider.CameraWasDisplaced(mainCam);
    }

    void RaiseOccludedCamPrio()
    {

        if (isPriorityChangedOnce == false)
        {
            occludedCam.Priority = occludedCam.Priority + 1;
            isPriorityChangedOnce = true;
        }


    }
    void LowerOccludedCamPrio()
    {
        if (isPriorityChangedOnce == true)
        {
            occludedCam.Priority = occludedCam.Priority - 1;
            isPriorityChangedOnce = false;

        }
    }
}
