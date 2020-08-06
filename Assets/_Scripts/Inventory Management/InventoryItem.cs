using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryItem : MonoBehaviour
{
    [SerializeField]
    bool isNearItem;

    public Inventory inventorSO;

    public string itemName;

    public GameObject inventoryObject;

    public AudioSource inventoryItemSound;

    private void Update()
    {
        if (isNearItem)
        {
            if (InputReceiver.CheckIf_Use_Pressed())
            {
                AddItemToInventory(itemName);
                inventoryObject.SetActive(false);
                //inventoryItemSound.PlayOneShot(inventoryItemSound.clip);
            }
        }
    }


    public void AddItemToInventory(string name)
    {
        inventorSO.AddItemToList(name);
    }

    private void OnTriggerStay(Collider other)
    {
        isNearItem = true;
    }
}
