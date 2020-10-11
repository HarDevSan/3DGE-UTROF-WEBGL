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
    }

    void LateUpdate()
    {
        /*When brain is blending from cam to another, we dont want any axis input, 
        *therefore we set the axis temporarily to empty strings*/
        if (brain.IsBlending)
        {
            vFreeLookCam.m_XAxis.m_InputAxisName = "";
            vFreeLookCam.m_YAxis.m_InputAxisName = "";
            Debug.Log("IS BLENDING");
        }

        //Check for Camera Displacement
        SwapTargetOnCollision(); 
    }

    void SwapTargetOnCollision()
    {
        vCamBase_MiddleRig = vFreeLookCam.GetRig(1);
        isCollsionGoingOn = cinemachineCollider.CameraWasDisplaced(vCamBase_MiddleRig);

        if (isCollsionGoingOn)
        {
            Debug.Log("Reached Cam Collision");
            vFreeLookCam.LookAt = alternateLookAt;
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

//Old unneeded code 
//    public CinemachineCollider vFreeLookCamCollider;

//    [Range(.5f, 1)]
//    public float clampYFactorPositve;
//    [Range(0f,.5f)]
//    public float clampYFactorNegative;

//    /*Fields for camra behaviour, as the cemarea target lookAt transform is slightly offset on negtive Z-axis,
//     * this causes problems with the Cinemachine Collider Strategy. Therefore, the first frame the vCam gets occluded,
//     * I switch the target to one that is centered inside the players body andhas no offset. This will cause Cinemachines
//     * collision strategy to work as intended. -- Does not work, no acces to ask if camera is occluded*/
//    [Header("Transfom when cam is not-occluded")]
//    public Transform transformWheNotOccluded;
//    [Header("Transfom when cam is occluded")]
//    public Transform transformWhenIsOccluded;

//    // Start is called before the first frame update
//    void Start()
//    {
//        //vFreeLookCam = GetComponent<CinemachineFreeLook>();
//        //vFreeLookCamCollider = vFreeLookCam.GetComponent<CinemachineCollider>();

//    }

//    void SwapTargets(Transform target, Vector3 PostionDelta)
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//        if(vFreeLookCam != null)
//        {
//            Debug.Log("Cam found!");
//        }

//        /*Check each frame if camera is occluded, if it is, swp target to non offset target. 
//        Otherwise, kep the centered one for the time of the occlusion.*/
//        if (vFreeLookCamCollider.IsTargetObscured(vFreeLookCam))
//        {
//            Debug.Log("Cam is Occluded");
//            vFreeLookCam.m_LookAt = transformWhenIsOccluded;
//        }
//        else
//        {
//            vFreeLookCam.m_LookAt = transformWheNotOccluded;
//            Debug.Log("Cam is NOT Occluded");

//        }


//        float axis = vFreeLookCam.m_YAxis.Value;

//        if(axis > clampYFactorPositve)
//        {
//            float lastpos = vFreeLookCam.m_YAxis.Value;

//            axis = Mathf.Max(axis, clampYFactorPositve);

//        }
//        else if(axis < clampYFactorNegative)
//        {
//            float lastpos = vFreeLookCam.m_YAxis.Value;

//            axis = Mathf.Max(axis, clampYFactorNegative);

//        }
//        vFreeLookCam.m_YAxis.Value = axis;

//    }
//    void SwitchLookAtTargetsOnOcclusion() {



//    }
//}
