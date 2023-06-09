﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScriptedEvent : MonoBehaviour
{
    /* Scripted Events are an intgral part of any playable 3D game envrionment. The game design theory is that any element like soundFX or visualFX 
    * shall occur when the player enters certain areas or spots of interest, or interacts. It could be something like a sound effect playing when the player enters a room
    * or opens a door. It could be that when pressing the "use" key on a console, the time of day changes. The possibilities are and should be endless in a flexible Scripted
    * Event System. The big advantage of using Unity in this context is that any components function can be called inside a dynamic list of functions attached to this
    * event. It separates the event triggering from the logic, which makes it highly versatile due to easily exchangable and chainable behaviour.
    */

    /* Serialized events in Unity makes this aspect of development natively accessable, because it propagates the complexity to the editor's hierarchy and inspector, instead
    * of the need to tightly couple elements through code.This makes it possible to modularly stack up these "Fear emitters". The emitter does not care what calls it. 
    * Any functioncan be used to trigger the Unit Event. The Unity Event an call any funtions assigned to it. The benefit of this approach is that seemingly complex dynam 
    * behavior and interactions in the game world can be created through minimal coupling. Furthermore it is non destructive through its modular approach, which offers a
    * lot of control at editing time.
    */
    public UnityEvent scriptedEvent;

    //List of events that can be wired to other functions to trigger THIS Unity Event
    public delegate void PlayerPressedUsedStartEmitter();
    public static event PlayerPressedUsedStartEmitter OnPlayerPressedUsedStartEmitter;

    //check if the player is in proimity via the trigger component's callback
    bool isPlayerInsideTrigger;
    bool isHasSeeOnce;

    [Header("Bools")]
    public bool shallDeactivateItselfAfterEvent;
    public bool isThisEventLookAtBased;

    [Header("Colliders")]
    public BoxCollider boxCollider;

    private void Start()
    {
        //See function for details
        InputReceiver.On_E_Input += StartScriptedEventOnPlayerPressedUse;
    }

    private void Update()
    {
        //Look based events need to take place in Update, cause it needs to be known if the player has looked at something.
        if (PlayerController.isPlayerCanInteractBecauseHeLooksAt_ScriptedEvent && isThisEventLookAtBased && isHasSeeOnce == false)
        {
            StartScriptedEvent();
            isHasSeeOnce = true;
        }
        else
        {
            //flip flop isHasSeenOnce bool
            if(isHasSeeOnce == true)
            isHasSeeOnce = false;
        }
    }

    void StartScriptedEvent()
    {
        scriptedEvent.Invoke();
        //Prevents a second trigger of the scripted event was already fired
       if(shallDeactivateItselfAfterEvent)
        DeactivateThisObject();
    }

    void DeactivateBoxCollider()
    {
        boxCollider.enabled = false;
    }

    void DeactivateThisObject()
    {
        gameObject.SetActive(false);
    }

    /* More specific example of using the emitter. It can catch other events to occur in the game world, e.g. the player pressing the use key at a specific location.
     * Then it can trigger Unity event to play whatever other functions or components are attached to it. This could be other function calls or component behaviour like
     * playing an animation clip.
     */
    void StartScriptedEventOnPlayerPressedUse()
    {
        if (isPlayerInsideTrigger)
        {
            scriptedEvent.Invoke();
        }
       // DeactivateThisObject();

    }


    /* This is the simplest form of execution. On entering the assigned trigger, all assigned functions or animations get fired.
     * More complex behaviour could be authored when other functions, see functions above. This behavior can be bypassed by simply not adding a trigger to THIS object.
     */
    private void OnTriggerEnter(Collider other)
    {
        scriptedEvent.Invoke();
       // DeactivateThisObject();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
            isPlayerInsideTrigger = true;
        else
        {
            isPlayerInsideTrigger = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            isPlayerInsideTrigger = false;
        else
        {
            isPlayerInsideTrigger = true;
        }
        //Make sure that once the player already was in the volume, THIS object and hence this collider will be disabled
        //DeactivateThisObject();

    }


}

