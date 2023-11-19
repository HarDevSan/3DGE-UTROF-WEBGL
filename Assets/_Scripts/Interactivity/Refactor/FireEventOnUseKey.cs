using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NodeCanvas.StateMachines;


public class FireEventOnUseKey : MonoBehaviour
{
    //public static Event OnPlayerInteractsHere;

    public FSMOwner owner;

    public void SendNormalEvent()
    {
        owner.SendEvent("MyEvent");
    }

    private void Update()
    {

        if (PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_item && InputReceiver.CheckIf_Use_Pressed())
        {
           // FireEvent();
            SendNormalEvent();
            Debug.Log("UseKeyPRessed");
        }
    }

    public void FireEvent()
    {
        //Debug.Log("ISEVENTNULL??? : " + (OnPlayerInteractsHere == null));
        //if (OnPlayerInteractsHere != null)
        //{
        //    OnPlayerInteractsHere.Invoke();
        //}
    }


}
