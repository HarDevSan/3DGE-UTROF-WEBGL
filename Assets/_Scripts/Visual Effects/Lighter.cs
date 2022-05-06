using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighter : MonoBehaviour
{
    public Light lighter;
    public InventorySearcher inventorySearcher;
    public static bool isLighterEnabled;

    private void Awake()
    {
        InputReceiver.On_F_Inpu += EnableLighter;
        InputReceiver.On_F_Second_Input += DisableLighter;

        lighter.enabled = false;
    }

    bool checkIfLighterIsInInvenotry()
    {
        return inventorySearcher.CheckIfItemIsInList("Lighter");
    }

    bool CheckIfGameIsPaused()
    {
        return GameManager.isGamePaused;
    }

    void EnableLighter()
    {
        if (CheckIfGameIsPaused() == false && checkIfLighterIsInInvenotry())
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
