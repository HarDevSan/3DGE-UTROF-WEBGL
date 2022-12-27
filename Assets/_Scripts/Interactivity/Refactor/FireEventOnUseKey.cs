using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class FireEventOnUseKey : MonoBehaviour
{
    public UnityEvent OnPlayerInteractsHere;


    private void OnTriggerStay()
    {

        if (PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_item && InputReceiver.CheckIf_Use_Pressed())
        {
            FireEvent();
            Debug.Log("UseKeyPRessed");
        }
    }

    public void FireEvent()
    {
        Debug.Log("ISEVENTNULL??? : " + (OnPlayerInteractsHere == null));
        if (OnPlayerInteractsHere != null)
        {
            OnPlayerInteractsHere.Invoke();
        }
    }


}
