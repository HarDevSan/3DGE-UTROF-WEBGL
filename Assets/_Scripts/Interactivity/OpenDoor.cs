using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OpenDoor: MonoBehaviour
{
    public Animator doorAnimator;

    private void Start()
    {
        InputReceiver.On_E_Input += PlayDoorAnim;
    }


    void PlayDoorAnim()
    {
        
            doorAnimator.SetTrigger("doorOpened");
        
    }

    
}
