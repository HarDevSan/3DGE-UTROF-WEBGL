using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
    public Transform childTransform;

    public virtual void OnTriggerEnter(Collider other)
    {
        childTransform.gameObject.SetActive(true);

    }
    public virtual void OnTriggerExit(Collider other)
    {
        childTransform.gameObject.SetActive(false);

    }
}
