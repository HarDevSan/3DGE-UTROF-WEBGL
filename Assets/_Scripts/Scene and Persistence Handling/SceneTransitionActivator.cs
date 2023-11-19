using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionActivator : MonoBehaviour
{
    //We wanna keep this for later when enemies are running around
    public LayerMask mask;

    public Transform childTransform;


    public virtual void OnTriggerStay(Collider other)
    {
        childTransform.gameObject.SetActive(true);

    }
    public virtual void OnTriggerExit(Collider other)
    {
        childTransform.gameObject.SetActive(false);

    }

}
