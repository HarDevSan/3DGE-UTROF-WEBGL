﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/Inventory", order = 1)]
public class Inventory : ScriptableObject
{
    public List<string> itemList;

    public bool SearchListFor(string name)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].Equals(name))
            {
                return true;

            }
            

        }

        return false;
    }

    public void AddItemToList(string name)
    {
        itemList.Add(name);
    }

}
