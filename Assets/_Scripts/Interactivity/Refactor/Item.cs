using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour, ICollectable
{
    public string itemName;

    public Inventory inventorySO;

    public void Collect()
    {
        inventorySO.AddItemToList(itemName);
    }
}
