using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FovIncreaser : MonoBehaviour
{
    public Camera cam;

    public void IncreaseFov()
    {
        cam.fieldOfView = 90;
    }
}
