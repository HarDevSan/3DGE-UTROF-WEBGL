using System.Collections;
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
    public TextMeshProUGUI miliSeconds;

    int fps;

    public Color greaterEqual60_Color;
    public Color greaterEqual30Below60_Color;
    public Color below30_Color;

    int counter;


    // Update is called once per frame
    void Update()
    {

            //Counter for the displaying interval
            counter++;
        

        Debug.Log(counter);

        //display only every 10th frame
        if (counter == 10)
        {

            //Calculate framerate and cast to int
            fps = (int)(1f / Time.smoothDeltaTime);
            fpsCounter.text = "FPS: " + fps;
            //Calculate miliseconds and cast to float
            miliSeconds.text = "MS: " + ((float)1 / fps) * 1000;
            miliSeconds.maxVisibleCharacters = 9;

            //Color coding depending on fps range
            if (fps >= 59)
            {
                fpsCounter.color = greaterEqual60_Color;
                miliSeconds.color = greaterEqual60_Color;

            }
            else if (fps < 60 && fps >= 30)
            {
                fpsCounter.color = greaterEqual30Below60_Color;
                miliSeconds.color = greaterEqual30Below60_Color;
            }
            else if (fps < 30)
            {
                fpsCounter.color = below30_Color;
                miliSeconds.color = below30_Color;
            }
            else
            {
                fpsCounter.color = below30_Color;
                miliSeconds.color = below30_Color;
            }
            //reset counter
            counter = 0;
        }
        
    }

}

