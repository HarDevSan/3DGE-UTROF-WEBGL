using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighter : MonoBehaviour
{
    public Light lighter;

    private void Awake()
    {
        InputReceiver.On_F_Inpu += EnableLighter;
        InputReceiver.On_F_Second_Input += DisableLighter;
    }

    void EnableLighter()
    {
        Debug.Log("FunctionReached");
        lighter.enabled = true;
    }
    void DisableLighter()
    {
        lighter.enabled = false;
    }


    private void OnEnable()
    {
        InputReceiver.On_F_Inpu += EnableLighter;
        InputReceiver.On_F_Second_Input += DisableLighter;
    }
    private void OnDisable()
    {
        InputReceiver.On_F_Inpu -= EnableLighter;
        InputReceiver.On_F_Second_Input -= DisableLighter;
    }
}
