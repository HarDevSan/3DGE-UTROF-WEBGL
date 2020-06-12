using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform nextTeleporter;
    public Transform intruder;

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Entered Trigger");
        gameObject.SetActive(false);
        nextTeleporter.gameObject.SetActive(true);

        Debug.Log("FuckFieHenne");
        TeleportIntruder();
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exited");
    }

    void TeleportIntruder() {

        intruder.position = nextTeleporter.position ;

    }
}
