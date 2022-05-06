using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/Inventory", order = 1)]
public class Inventory : ScriptableObject
{
    public List<string> itemList;

    public void AddItemToList(string name)
    {
        Debug.Log("Added item to inventory called : " + name);
        itemList.Add(name);
    }


}

