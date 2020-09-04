using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionActivator : MonoBehaviour
{

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
