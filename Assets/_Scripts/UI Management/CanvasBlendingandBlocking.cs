using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBlendingandBlocking : MonoBehaviour
{

    [Header("Canvasses")]
    public CanvasGroup interactionCanvasGroup;
    public CanvasGroup settingsCgroup;

    public delegate void SettingsBlendedIn();
    public static event SettingsBlendedIn OnSettingsBlendedIn;
    public delegate void SettingsBlendedOut();
    public static event SettingsBlendedOut OnSettingsBlendedOut;

    public UIBlending uiBLendingSO;

    private void Awake()
    {
        InputReceiver.On_P_Input += BlendInSettings;
        InputReceiver.On_P_Second_Input += BlendOutSettings;

        GameManager.OnGameHasBeenPaused += DisableInteractionCanvasBlockRayCasts;
        GameManager.OnGameHasBeenPaused += EnableSettingsCanvasBlockRayCasts;
        GameManager.OnGameHasBeenResumed += DisableSettingsCanvasBlockRayCasts;
        GameManager.OnGameHasBeenResumed += EnableInteractionCanvasBlockRayCasts;
    }

    private void Start()
    {
        DisableSettingsCanvasBlockRayCasts();

    }
    public void EnableInteractionCanvasBlockRayCasts()
    {
        interactionCanvasGroup.blocksRaycasts = true;

    }

    public void DisableInteractionCanvasBlockRayCasts()
    {
        interactionCanvasGroup.blocksRaycasts = false;

    }

    public void EnableSettingsCanvasBlockRayCasts()
    {
        settingsCgroup.blocksRaycasts = true;

    }

    public void DisableSettingsCanvasBlockRayCasts()
    {
        settingsCgroup.blocksRaycasts = false;

    }

    void BlendInSettings()
    {
        StartCoroutine(BlendInSettingsRoutine());
    }

    void BlendOutSettings()
    {
        StartCoroutine(BlendOutSettingsRoutine());
    }

    IEnumerator BlendInSettingsRoutine()
    {
        float from = 0f;
        float to = 1f;
        float t = 0f;
        float lerpvalue = 0; 

        while (lerpvalue < to)
        {
            t += uiBLendingSO.toSettingsBlendTime;
            lerpvalue = Mathf.Lerp(from, to, t);
            settingsCgroup.alpha = lerpvalue;
            yield return null;
            Debug.Log(lerpvalue);
        }
        OnSettingsBlendedIn.Invoke();
    }

    IEnumerator BlendOutSettingsRoutine()
    {
        float from = 1f;
        float to = 0f;
        float t = 0f;
        float lerpvalue = 1;

        while (lerpvalue > to)
        {
            t += uiBLendingSO.toSettingsBlendTime;
            lerpvalue = Mathf.Lerp(from, to, t);
            settingsCgroup.alpha = lerpvalue;
            yield return null;

        }
        OnSettingsBlendedOut.Invoke();

    }


}
