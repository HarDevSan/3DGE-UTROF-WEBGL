using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionActivator : MonoBehaviour
{
    public float sphereRadius;

    public LayerMask mask;

    public Transform child;

    private void FixedUpdate()
    {
        //Overlap Automatically limits these calls to when the player line of sight sees somehting interactable , in this case a door

        //Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius, mask);


        //if (colliders.Length > 0)
        //{
        //    for (int i = 0; i < colliders.Length; i++)
        //    {
        //        if (colliders[i].gameObject.tag == "Player")
        //        {
        //            child.gameObject.SetActive(true);
        //            Debug.Log("Player Hit");
        //        }

        //    }
        //}
        //else
        //    child.gameObject.SetActive(false);



    }

    private void OnTriggerEnter(Collider other)
    {
        child.gameObject.SetActive(true);

    }
    private void OnTriggerExit(Collider other)
    {
        child.gameObject.SetActive(false);

    }

}
