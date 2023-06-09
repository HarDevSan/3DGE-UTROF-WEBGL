using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TemporaryMovementStopper : MonoBehaviour
{
    public CharacterController charController;

    void DisableController()
    {
        charController.enabled = false;
    }

    void EnableController()
    {
        charController.enabled = true;
    }
}
