using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryResetter : MonoBehaviour
{
    public Inventory inventoryToResetSO;

    // Start is called before the first frame update
    void Start()
    {
        inventoryToResetSO.itemList = new List<string>();
    }

   
}
