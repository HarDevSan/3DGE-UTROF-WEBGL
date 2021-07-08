using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamPermanent : MonoBehaviour
{
    Transform mainCamTrans;
    [Range(.01f, 3)]
    public float rotSpeed;

    public Transform toTransform;
    float initialX;

    // Start is called before the first frame update
    void Start()
    {
        mainCamTrans = Camera.main.transform;
        initialX = toTransform.rotation.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion newRot = new Quaternion();

        newRot = Quaternion.Slerp(toTransform.rotation, Quaternion.LookRotation(mainCamTrans.forward), Time.deltaTime * rotSpeed);
        toTransform.rotation = newRot;
        toTransform.eulerAngles = new Vector3(initialX, toTransform.eulerAngles.y, 0);

    }
}
