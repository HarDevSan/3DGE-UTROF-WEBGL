using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Act_DeactivteUIElementOnEnter_Exit : MonoBehaviour
{
    public GameObject gameObjectToActivate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            gameObjectToActivate.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            gameObjectToActivate.SetActive(false);
    }

}
