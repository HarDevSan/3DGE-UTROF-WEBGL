using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class rotates an UI element towards the main cameras forward vector only for the time the player stays inside this trigger
public class RotateToCamera : MonoBehaviour
{

    Transform mainCamTrans;
    [Range(.01f, 3)]
    public float rotSpeed;

    public Transform toTransform;
    float initialX;

    void Start()
    {
        mainCamTrans = Camera.main.transform;
        initialX = toTransform.rotation.eulerAngles.x;

    }

    //Only calculate roation when player inside trigger. Prevents use of Update.
    private void OnTriggerStay(Collider other)
    {

        Quaternion newRot = new Quaternion();


        if (other.tag == "Player")
        {

            newRot = Quaternion.Slerp(toTransform.rotation, Quaternion.LookRotation(mainCamTrans.forward), Time.deltaTime * rotSpeed);
            toTransform.rotation = newRot;
            toTransform.eulerAngles = new Vector3(initialX, toTransform.eulerAngles.y, 0);
        }
    }

}
