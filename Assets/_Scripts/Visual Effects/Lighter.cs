using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighter : MonoBehaviour
{
    public GameObject lighter;
    public InventorySearcher inventorySearcher;
    public static bool isLighterEnabled;

    public GameObject tempFlame;

    private void Awake()
    {
        InputReceiver.On_F_Inpu += EnableLighter;
        InputReceiver.On_F_Second_Input += DisableLighter;

        lighter.SetActive(false);
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
        {
            lighter.SetActive(true);
            if(tempFlame != null)
            tempFlame.SetActive(true);
            isLighterEnabled = true;
        }
    }
    void DisableLighter()
    {
        lighter.SetActive(false);
        isLighterEnabled = false;
        if (tempFlame != null)
        tempFlame.SetActive(false);

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
