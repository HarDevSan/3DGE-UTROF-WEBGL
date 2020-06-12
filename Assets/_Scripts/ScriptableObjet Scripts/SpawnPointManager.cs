using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SpawnPointManager", menuName ="ScriptableObjects/SpawnPointManagement", order = 1)]
public class SpawnPointManager : ScriptableObject
{

    public Transform spawnPoint_Room_1_toMainHall;
    public Transform spawnPoint_MainHall_to_Room_1;


    public void GetSpawnPointByName(string name)
    {


    }
    


    

}
