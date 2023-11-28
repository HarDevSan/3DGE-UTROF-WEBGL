using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyJitter : MonoBehaviour
{
    public Light light;
    [Range(1,3)]
    public float lightRandomangeMin;
    [Range(2, 4)]
    public float lightRandomangeMax;

    public Vector3 lightDefaultPos;
    public Vector3 randomRangeAdd;

    public Lighter lighter;

    private void Start()
    {
        lightDefaultPos = light.transform.localPosition;
    }


    // Update is called once per frame
    void Update()
    {
        if (Lighter.isLighterEnabled)
        {
            RandomizeLightRange();
            RandomizeLightPos();
        }
    }

    void RandomizeLightPos()
    {
        //ADD value on top of the maximum random value
        light.transform.localPosition = new Vector3(Random.Range(lightDefaultPos.x, lightDefaultPos.x + randomRangeAdd.x),
                                            Random.Range(lightDefaultPos.y, lightDefaultPos.y +  randomRangeAdd.y),
                                            Random.Range(lightDefaultPos.z, lightDefaultPos.z +  randomRangeAdd.z));
    }

    void RandomizeLightRange()
    {
        light.intensity = Random.Range(lightRandomangeMin, lightRandomangeMax);

    }
}
