using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScriptedEvent : MonoBehaviour
{
    public UnityEvent scriptedEvent;
    public bool isAlreadyTriggered;

    public virtual void InvokeScriptedEvent()
    {
        scriptedEvent.Invoke();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (isAlreadyTriggered == false)
            InvokeScriptedEvent();
        isAlreadyTriggered = true;

    }
}
