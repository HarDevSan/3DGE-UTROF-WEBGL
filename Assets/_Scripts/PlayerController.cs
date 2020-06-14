using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{


    public float _toDefaultWalkSpeedLerpTime;
    public float _toDefaultStrafeSpeedLerpTime;

    public Transform resetTo;
    [Header("RayTarget")]
    public Transform castRayFrom;

 
    public Camera _mainCamera;
    public Animator playerAnimator;

    public static bool isApplyGravity;
    public static bool isPlayerCanInteractBecauseHeLooksAtSmth;

    public float interactionDistance;

    [Header("Interaction LayerMask")]
    public LayerMask interactionMask;

    CharacterController _characterController;

    Vector3 _velocity;
    Vector3 inputVectorWASD;

    public delegate void PlayerSeesSomehtingInteractable();
    public static event PlayerSeesSomehtingInteractable OnPlayerSeesSomethingInteractable;
    public delegate void PlayerDoesNotSeeSomehtingInteractable();
    public static event PlayerDoesNotSeeSomehtingInteractable OnPlayerSeesSoPlayerDoesNotSeeSomehtingInteractable;

    //SO
    public PlayerStats playerstats;

    private void Awake()
    {
        InputReceiver.On_R_Input += ResetPlayer;
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();

        //Set default speed values for walking and strafing, which we then lerp back to in the Run() method, when the player stopped running
        playerstats._defaultWalkSpeed = playerstats._walkSpeed;
        playerstats._defaultStrafeSpeed = playerstats._strafeSpeed;
    }

    void Update()
    {
        //Gravity needs to always be applied, even if we have no jumping but maybe some falling
        if(isApplyGravity)
        ApplyGravity();

        AnimatePlayer();

        //Only call Movement related functions if the player does movmeent input
        if (InputManager.CheckIfAnyMovementInput())
        {

            MovePlayerWASD();

            //Only call Run function if player is holding run key
            if (InputReceiver.CheckIf_Run_Pressed())
            { 
                Run();

            }

            TurnPlayer();
        }

        //Poll movemenVector from InputReceiver
        inputVectorWASD = InputReceiver.movementInput.normalized; //Normalize to disable speed advantage


        //RayCasting
        CastRayFromFront();
    }

  
    void ApplyGravity()
    {
        //Check if the player is grounded and turn off the gravity in that case
        if (_characterController.isGrounded)
        {
            _velocity = Vector3.zero;
            //Debug.Log("grounded");
        }
        else
        {
            //If the player is not grounded, move him downwards on the y-axis. Note this is NOT the same as applying force with a rigidbody as we are using a character controller
            _velocity = new Vector3(_velocity.x, _velocity.y - playerstats._gravityStrength * Time.deltaTime, _velocity.z);
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }

    void MovePlayerWASD()
    {

        //Zero-ed out y of camera forward and right vectors, preventing slow down through y-axis taken into account
        Vector3 camForwardZeroY = new Vector3(_mainCamera.transform.forward.x, 0f, _mainCamera.transform.forward.z);
        Vector3 camRightZeroY = new Vector3(_mainCamera.transform.right.x, 0f, _mainCamera.transform.right.z);

        if (InputManager.CheckIfVerticalInput() == false)
        {
            if (inputVectorWASD.y < 0)
            {
                playerstats._walkSpeed = playerstats._walkBackwardsSpeed;
            }
            else if (inputVectorWASD.y > 0)
            {
                playerstats._walkSpeed = playerstats._defaultWalkSpeed;
            }
        }
        else
        {
            if (inputVectorWASD.y < 0)
            {
                playerstats._walkSpeed = playerstats._walkBackwardsSpeed;
            }
            else if (inputVectorWASD.y > 0)
            {
                Run();
            }
        }
        //Forwards Backwards Movements
        _characterController.Move(camForwardZeroY * inputVectorWASD.y * playerstats._walkSpeed * Time.deltaTime);
        //Strafing
        _characterController.Move(camRightZeroY * inputVectorWASD.x * playerstats._strafeSpeed * Time.deltaTime);
        //FUNNY we could simply swap x and y here to make a classical "spellbound" like effect on the player when he is hit by some kind of poison or spell

    }


    public void Run()
    {
        //Run forward
        if (InputManager.CheckIfVerticalInput())
        {
            playerstats._walkSpeed = Mathf.Lerp(playerstats._walkSpeed, playerstats._runSpeed, playerstats._toRunLerpTime * Time.deltaTime);
        }
        else
        {
            playerstats._walkSpeed = Mathf.Lerp(playerstats._walkSpeed, playerstats._defaultWalkSpeed, _toDefaultWalkSpeedLerpTime * Time.deltaTime);
        }

        //Run sidewards
        if (InputManager.CheckIfHorizontalInput())
        {
            playerstats._strafeSpeed = Mathf.Lerp(playerstats._strafeSpeed, playerstats._strafeRunSpeed, playerstats._toStrafeRunLerpTime * Time.deltaTime);
        }
        else
        {
            playerstats._strafeSpeed = Mathf.Lerp(playerstats._strafeSpeed, playerstats._defaultStrafeSpeed, _toDefaultStrafeSpeedLerpTime * Time.deltaTime);
        }

    }

    void TurnPlayer()
    {

        //Cast Ray from the middle of the screen
        Ray directionFromScreen = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        //Get the direction of the ray to slerp towards, so play will always believbly look forward

        //local varaible to hold the direction vector from screen center into world
        Vector3 direction = directionFromScreen.direction;

        //Vector3 direction = _mainCamera.transform.position - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, playerstats._turnSpeed * Time.deltaTime);

        //Zero out any rotations on x and z
        var zeroedOutEulers = transform.eulerAngles;
        zeroedOutEulers.x = 0;
        zeroedOutEulers.z = 0;
        transform.eulerAngles = zeroedOutEulers;


    }

    void CastRayFromFront()
    {
        RaycastHit hit;
        if(Physics.Raycast(castRayFrom.position, castRayFrom.forward, out hit, interactionDistance, interactionMask)){

            //Debug.Log(isPlayerCanInteractBecauseHeLooksAtSmth);
            OnPlayerSeesSomethingInteractable.Invoke();
            isPlayerCanInteractBecauseHeLooksAtSmth = true;
        }
        else
        {
            //Debug.Log(isPlayerCanInteractBecauseHeLooksAtSmth);
            OnPlayerSeesSoPlayerDoesNotSeeSomehtingInteractable.Invoke();
            isPlayerCanInteractBecauseHeLooksAtSmth = false;
        }
        Debug.DrawRay(castRayFrom.position, castRayFrom.forward, Color.green);

    }

    void ResetPlayer()
    {
        {
            transform.position = resetTo.position;
        }
    }

    public void AnimatePlayer()
    {
        playerAnimator.SetFloat("WalkSpeed", InputReceiver.movementInput.magnitude);
    }

    void SetPlayerToPlayableState()
    {
        isApplyGravity = true;
    }
    void SetPlayerToUnplayableState()
    {
        isApplyGravity = true;
    }
}
