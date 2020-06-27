using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class FearEmitter : MonoBehaviour
{

    public UnityEvent playerEntersEmitter;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered Emitter");
        playerEntersEmitter.Invoke();
    }

}
