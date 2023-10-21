using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class SliderVolumeController : MonoBehaviour
{
    public Slider slider;

    public AudioMixer mixer;

    public void ChangeMusicVolume()
    {
        mixer.SetFloat("Music", Mathf.Log10(slider.value) * 20);
    }
}
