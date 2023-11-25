using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetRotator : MonoBehaviour
{
    public PlayerStats stats;    // Update is called once per frame
    public float maxVerticalAnglePositive;
    public float maxVerticalAngleNegative;


    void Update()
    {
        RotateFollowTarget();
        ClampFollowTargetAngles();
    }

    void RotateFollowTarget()
    {
        transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * stats.mouseSpeed * Time.deltaTime, Vector3.up);
        transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * stats.mouseSpeed * Time.deltaTime, Vector3.left);

        var zeroedOutEulers = transform.eulerAngles;
        zeroedOutEulers.z = 0;
        transform.eulerAngles = zeroedOutEulers;
    }

    void ClampFollowTargetAngles()
    {
        Vector3 angles = transform.localEulerAngles;
        float angle = transform.localEulerAngles.x;

        if (angle > maxVerticalAnglePositive && angle < 290)
        {
            angles.x = maxVerticalAnglePositive;
        }
        else if (angle < maxVerticalAngleNegative && angle > 290)
        {
            angles.x = maxVerticalAngleNegative;
        }
        transform.localEulerAngles = new Vector3(angles.x, angles.y, 0);
    }
}
