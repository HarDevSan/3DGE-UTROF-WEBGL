using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ToBeContinued : MonoBehaviour
{
    public GameObject TBC;

    private void OnTriggerEnter(Collider other)
    {
        TBC.SetActive(true);
    }
}
