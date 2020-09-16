using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScriptedEvent : MonoBehaviour
{
    public UnityEvent scriptedEvent;

    public void InvokeScriptedEvent()
    {
        scriptedEvent.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        InvokeScriptedEvent();  
    }
}
