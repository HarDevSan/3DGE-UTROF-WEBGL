using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReceiver : MonoBehaviour
{

    public static Vector2 movementInput;


    public static bool isRKey;
    public static bool isShiftKeyPressed;
    public static bool isEnterKeyPressed;

    public float moveSmoothDampTime;

    //Velocities
    Vector2 currentMoveMentInputVelocity = Vector3.zero;

    //-----------Events
    public delegate void InputEvent_E();
    public static event InputEvent_E On_E_Input;
    public delegate void InputEvent_R();
    public static event InputEvent_R On_R_Input;
    public delegate void InputEvent_P();
    public static event InputEvent_P On_P_Input;
    public delegate void InputEvent_P_Second();
    public static event InputEvent_P_Second On_P_Second_Input;

    public static bool isMovementInput;
    public static bool pauseWasPressed;

    private void Awake()
    {
        isMovementInput = true;
    }

    void Update()
    {
        if (isMovementInput)
        {
            ReceiveMovementInputWASD();
        }
        //Debug.Log("isMovementInput : " + isMovementInput);
        Check_If_PausePressed();
    }


    void ReceiveMovementInputWASD()
    {
        //Debug.Log(movementInput);
        //if(isMovementInput)
        movementInput = new Vector2(Input.GetAxis("Horizontal"), (Input.GetAxis("Vertical")));

    }

    void ClampYInput()
    {
        //No need, done in CineMachine FreeLook component
    }

    public void ClearAllInputs()
    {
    }

    public static void UnBlockMovementInputs()
    {
        isMovementInput = true;
    }

    public static void BlockMovementInput()
    {
        isMovementInput = false;
        movementInput = Vector2.zero;
    }


    public static bool CheckIf_Run_Pressed()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void CheckIf_ResetPlayer_Pressed()
    {
        if (Input.GetKey(KeyCode.R))
        {
            On_R_Input.Invoke();
        }
    }

    public static bool CheckIf_Use_Pressed()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // On_E_Input.Invoke(); No Subs, yet
            return true;
        }
        else
            return false;
    }

    public static bool Check_If_PausePressed()
    {
        if (pauseWasPressed == false)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {

                pauseWasPressed = true;
                On_P_Input.Invoke();

                return true;
            }
        }

        else if (Input.GetKeyDown(KeyCode.P))
        {
            pauseWasPressed = false;
            On_P_Second_Input.Invoke();

            return false;
        }

        return false;
    }


}
