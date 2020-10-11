using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStorm : MonoBehaviour
{
    Material material;
    Color materialEmissiveColor;
    Color thunderEmissiveColor;

    private void Start()
    {
        material = GetComponent<Material>();
        materialEmissiveColor = material.GetColor("_EmissionColor");
    }

    private void Update()
    {
        Color tempCol = new Color();
        float thunder = Mathf.PingPong(Time.time, thunderEmissiveColor.a);
        tempCol.a = thunder;
        material.SetColor("_EmissionColor", tempCol); 
    }
}
