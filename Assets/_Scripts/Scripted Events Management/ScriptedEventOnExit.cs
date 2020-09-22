using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedEventOnExit : ScriptedEvent
{

    public override void OnTriggerEnter(Collider other)
    {
        //Do nothing in this case
    }

    private void OnTriggerExit(Collider other)
    {
        InvokeScriptedEvent();
    }
}
