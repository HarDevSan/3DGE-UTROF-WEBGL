using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionActivator : MonoBehaviour
{

    public LayerMask mask;

    public Transform child;


    public virtual void OnTriggerStay(Collider other)
    {
        child.gameObject.SetActive(true);

    }
    public virtual void OnTriggerExit(Collider other)
    {
        child.gameObject.SetActive(false);

    }

}
