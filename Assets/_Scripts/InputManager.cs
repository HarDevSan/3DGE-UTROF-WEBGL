using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{


    private void Start()
    {

    }


    public static bool CheckIfAnyMovementInput()
    {

        if (InputReceiver.movementInput.x != 0 || InputReceiver.movementInput.y != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool CheckIfHorizontalInput()
    {

        if (InputReceiver.movementInput.x > 0 || InputReceiver.movementInput.x < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool CheckIfVerticalInput()
    {

        if (InputReceiver.movementInput.y > 0 || InputReceiver.movementInput.y < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }




}
