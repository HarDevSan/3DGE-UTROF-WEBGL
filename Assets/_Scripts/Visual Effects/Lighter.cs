using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighter : MonoBehaviour
{
    public Light lighter;
    public Inventory inventory;
    public static bool isLighterEnabled;

    private void Awake()
    {
        InputReceiver.On_F_Inpu += EnableLighter;
        InputReceiver.On_F_Second_Input += DisableLighter;

        lighter.enabled = false;
    }

    /*Check if Lighter is already collected by the player and added to 
     * the list of names in the Inventory Scriptable Object*/
    bool checkIfLighterIsInInvenotry()
    {
        if (inventory.SearchListFor("Lighter"))
        {
            return true;
        }
        else
        {
            return false;
        }
            
    }

    bool CheckIfGameIsPaused()
    {
       return GameManager.isGamePaused;
    }

    void EnableLighter()
    {   if(CheckIfGameIsPaused() == false)
        lighter.enabled = true;
        isLighterEnabled = true;
    }
    void DisableLighter()
    {
        lighter.enabled = false;
        isLighterEnabled = false;
    }


    private void OnEnable()
    {
        InputReceiver.On_F_Inpu += EnableLighter;
        InputReceiver.On_F_Second_Input += DisableLighter;
    }
    private void OnDisable()
    {
        InputReceiver.On_F_Inpu -= EnableLighter;
        InputReceiver.On_F_Second_Input -= DisableLighter;
    }
}
