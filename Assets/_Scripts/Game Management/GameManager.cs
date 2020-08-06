using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;
using Cinemachine;

public class GameManager : MonoBehaviour
{

    public CharacterController characterControllerRef;

    public CinemachineBrain brain;

    public static bool isCharControllerEnabled;

    [Header("Canvasses")]
    public CanvasGroup interactionCanvasGroup;
    public CanvasGroup settingsCgroup;

    public float toSettingsBlendTime;

    private void Awake()
    {
        SceneLoader.OnSceneIsLoading += SetIsCharControllerEnabledToDisabled;
        SceneLoader.OnScene_Has_Loaded += SetIsCharControllerEnabledToEnabled;
        //Can't pause game when coroutines needs to run
        //InputReceiver.On_P_Input += PauseGame;
        InputReceiver.On_P_Second_Input += ResumeGame;
        InputReceiver.On_P_Input += BlendInSettingsAndPauseGame;
        InputReceiver.On_P_Second_Input += BlendOutSettings;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;

        isCharControllerEnabled = true;
        LockCursor();
        interactionCanvasGroup.interactable = false;
        settingsCgroup.interactable = false;
    }

    private void Update()
    {

        characterControllerRef.enabled = isCharControllerEnabled;
        Debug.Log("Character Conroller is enabled : characterControllerRef.enabled");

    }

    public static void SetIsCharControllerEnabledToEnabled()
    {
        isCharControllerEnabled = true;
        InputReceiver.isMovementInput = true;
    }

    public static void SetIsCharControllerEnabledToDisabled()
    {
        isCharControllerEnabled = false;
        InputReceiver.isMovementInput = false;
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        InputReceiver.BlockMovementInput();
        brain.enabled = false;
        settingsCgroup.interactable = true;
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        InputReceiver.UnBlockMovementInputs();
        brain.enabled = true;
        settingsCgroup.interactable = false;

    }

    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }

    public static void UnLockCursor()
    {
        Cursor.lockState = CursorLockMode.None;

    }

    void BlendInSettingsAndPauseGame()
    {
        StartCoroutine(BlendInSettingsAndPauseGameRoutine());
        UnLockCursor();
    }

    void BlendOutSettings()
    {
        StartCoroutine(BlendOutSettingsRoutine());
        LockCursor();
    }

    IEnumerator BlendInSettingsAndPauseGameRoutine()
    {
        while (settingsCgroup.alpha < .999f)
        {
            settingsCgroup.alpha = Mathf.Lerp(settingsCgroup.alpha, 1, toSettingsBlendTime * Time.deltaTime);
            yield return null;
        }

        PauseGame();

    }

    IEnumerator BlendOutSettingsRoutine()
    {

        while (settingsCgroup.alpha > .001f)
        {
            settingsCgroup.alpha = Mathf.Lerp(settingsCgroup.alpha, 0, toSettingsBlendTime * Time.deltaTime);
            yield return null;
        }

    }

}
