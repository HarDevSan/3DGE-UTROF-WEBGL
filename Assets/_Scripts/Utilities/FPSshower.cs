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
    public TextMeshProUGUI secondsGUI;

    public int maxSecondsToRecalcAverage;

    int avgFPS;

    public Color greaterEqual60_Color;
    public Color greaterEqual30Below60_Color;
    public Color below30_Color;

    int frameCounter;
    int seconds;

    private void Start()
    {
        StartCoroutine(countEverySecond());
    }

    void Update()
    {
        calcAVGFPS();
    }


    void calcAVGFPS()
    {
        //Count every frame
        frameCounter++;

        //Display the seconds elapsed until next measurement interval
        secondsGUI.text = "SEC: " + seconds;

        //display only every fpsInterval frame
        if (seconds >= maxSecondsToRecalcAverage)
        {

            //Calculate current framerate and cast to int
            //fps = (int)(1f / Time.smoothDeltaTime);

            //calculate averaged framerate over maxSeconds interval
            avgFPS = frameCounter / maxSecondsToRecalcAverage;
            fpsCounter.text = "AVG FPS: " + avgFPS;

            //Calculate miliseconds and cast to float
            miliSeconds.text = "MS: " + ((float)1 / avgFPS) * 1000;
            miliSeconds.maxVisibleCharacters = 9;


            //Color coding depending on fps range
            if (avgFPS >= 59)
            {
                fpsCounter.color = greaterEqual60_Color;
                miliSeconds.color = greaterEqual60_Color;
            }
            else if (avgFPS < 60 && avgFPS >= 30)
            {
                fpsCounter.color = greaterEqual30Below60_Color;
                miliSeconds.color = greaterEqual30Below60_Color;
            }
            else if (avgFPS < 30)
            {
                fpsCounter.color = below30_Color;
                miliSeconds.color = below30_Color;
            }
            else
            {
                fpsCounter.color = below30_Color;
                miliSeconds.color = below30_Color;
            }
            //reset counter and seconds after interval
            frameCounter = 0;
            seconds = 0;
        }
    }

    IEnumerator countEverySecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            seconds++;
        }
    }

}




