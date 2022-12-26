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
            Debug.Log("UseKeyPRessed");
            FireEvent();
        }
    }

    public void FireEvent()
    {
        if(OnPlayerInteractsHere != null)
        OnPlayerInteractsHere.Invoke();
    }


}
