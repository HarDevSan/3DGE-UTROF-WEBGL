using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CamStateBroadCaster", menuName = "ScriptableObjects/CamStateBroadCaster", order = 1)]

public class CamStateBroadcaster : ScriptableObject
{
    //public UnityEvent OnCinematicStateChanged;
    public bool isCinematicState;

    private void OnEnable()
    {
        //Put it to falsei n Awake to prevent accidental testing tick
        isCinematicState = false;
    }
    public void SwapCinematicState()
    {
        isCinematicState = !isCinematicState;
    }
       
}
