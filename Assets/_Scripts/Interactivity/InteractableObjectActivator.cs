using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectActivator : MonoBehaviour
{
    public GameObject hierarchyToActivate;
    public Collider boxCollider;

    private void OnTriggerEnter(Collider other)
    {
        hierarchyToActivate.SetActive(true);
        boxCollider.enabled = false;



    }




}
