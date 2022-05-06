using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySearcher : MonoBehaviour
{
    public Inventory inventory;
    bool isItemInList;

    public bool CheckIfItemIsInList(string name)
    {
        StartCoroutine(CheckIfItemsIsInListRoutine(name));
        return isItemInList;
    }

    IEnumerator CheckIfItemsIsInListRoutine(string name)
    {
        int counter = 0;

        while (counter < inventory.itemList.Count)
        {
            if (inventory.itemList[counter] == name)
                isItemInList = false;
            yield return null;
        }
        isItemInList = true;
        yield return null;
    }
}
