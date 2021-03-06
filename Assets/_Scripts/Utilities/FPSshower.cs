﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*This script is written by investigating the Unity Community Wiki
 * and the research for the formula to calculate FPS, which is simply "1/Time.deltaTime",
 * which mans 1 second divided b the time past since last frame. Color coding was added 
 * to be able to better evauate the outome*/

public class FPSshower : MonoBehaviour
{
    public TextMeshProUGUI fpsCounter;
    int fps;

    public Color greaterEqual60_Color;
    public Color greaterEqual30Below60_Color;
    public Color below30_Color;

    // Update is called once per frame
    void Update()
    {

        //Calculate framerate and cast to int
        fps = (int)(1f / Time.smoothDeltaTime);
        fpsCounter.text = "FPS: " + fps;

        //Color coding depending on fps range
        if (fps >= 59)
        {
            fpsCounter.color = greaterEqual60_Color;
        }
        else if (fps < 60 && fps >= 30)
        {
            fpsCounter.color = greaterEqual30Below60_Color;
        }
        else if (fps < 30)
        {
            fpsCounter.color = below30_Color;
        }
        else
        {
            fpsCounter.color = below30_Color;
        }

    }
}
