using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LookTargetSwitcher : MonoBehaviour
{
    public CinemachineFreeLook freeLookCam;
    public Transform newTemporarLookTarget;

    public void SwapLookTargetForSeconds()
    {
        freeLookCam.LookAt = newTemporarLookTarget;
    }
}
