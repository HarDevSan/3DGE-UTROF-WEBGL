using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;
    public UnityEvent eventResponse;

    public void OnEventRaised()
    {
        eventResponse.Invoke();
    }
    private void Start()
    {

        gameEvent.RegisterListener(this);
    }
    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        gameEvent.UnRegisterListener(this);
    }
}
