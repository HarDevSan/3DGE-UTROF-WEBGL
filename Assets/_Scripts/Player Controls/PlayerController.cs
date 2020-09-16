using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public Transform resetTo;
    [Header("RayTarget")]
    public Transform castRayFrom;

    public Camera _mainCamera;
    public Animator playerAnimator;

    public static bool isApplyGravity;
    public static bool isPlayerCanInteractBecauseHeLooksAtSmth_Room;
    public static bool isPlayerCanInteractBecauseHeLooksAtSmth_item;

    [Header("Interaction LayerMasks")]
    public LayerMask interactionMaskRoom;
    public LayerMask interactionMaskItem;

    CharacterController _characterController;

    Vector3 _velocity;
    Vector3 inputVectorWASD;

    public delegate void PlayerSeesSomehtingInteractable_Room();
    public static event PlayerSeesSomehtingInteractable_Room OnPlayerSeesSomethingInteractable_Room;
    public delegate void PlayerSeesSomehtingInteractable_Item();
    public static event PlayerSeesSomehtingInteractable_Item OnPlayerSeesSomethingInteractable_Item;
    public delegate void PlayerDoesNotSeeSomehtingInteractable();
    public static event PlayerDoesNotSeeSomehtingInteractable OnPlayerDoesNotSeeSomehtingInteractable;

    //Scriptable Objects
    public PlayerStats playerstats;

    static bool isCharControllerEnabled;



    private void Awake()
    {
        InputReceiver.On_R_Input += ResetPlayer;
        //Character Controller class must be enabled/diabled inside the update function for it work
        SceneLoader.OnSceneStartedLoading += SetPlayerToUnplayableState;
        SceneLoader.OnScene_Has_Loaded += SetPlayerToPlayableState;
        GameManager.OnGameHasBeenPaused += SetPlayerToUnplayableState;
        GameManager.OnGameHasBeenResumed += SetPlayerToPlayableState;
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        SetPlayerToUnplayableState();
        //Set default speed values for walking and strafing, which we then lerp back to in the Run() method, when the player stopped running
        playerstats._defaultWalkSpeed = playerstats._walkSpeed;
        playerstats._defaultStrafeSpeed = playerstats._strafeSpeed;
    }

    void Update()
    {
        //Gravity needs to always be applied, even if we have no jumping but maybe some falling
        // if (isApplyGravity)
        ApplyGravity();

        AnimatePlayer();

        //Set CharController enabled by member boo, cause must be done every frame

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
        else if(isApplyGravity)
        {
            //If the player is not grounded, move him downwards on the y-axis. Note this is NOT the same as applying force with a rigidbody as we are using a character controller
            _velocity = new Vector3(_velocity.x, _velocity.y - playerstats._gravityStrength * Time.deltaTime, _velocity.z);
            _characterController.Move(_velocity * Time.deltaTime);
        }
        else
        {
            _velocity = Vector3.zero;
        }
    }

    void MovePlayerWASD()
    {

        //Zero-ed out y of camera forward and right vectors, preventing slow down through y-axis taken into account
        Vector3 camForwardZeroY = new Vector3(_mainCamera.transform.forward.x, 0f, _mainCamera.transform.forward.z).normalized;
        Vector3 camRightZeroY = new Vector3(_mainCamera.transform.right.x, 0f, _mainCamera.transform.right.z).normalized;

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
        _characterController.Move(camForwardZeroY * inputVectorWASD.normalized.y * playerstats._walkSpeed * Time.deltaTime);
        //Strafing
        _characterController.Move(camRightZeroY * inputVectorWASD.normalized.x * playerstats._strafeSpeed * Time.deltaTime);
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
            playerstats._walkSpeed = Mathf.Lerp(playerstats._walkSpeed, playerstats._defaultWalkSpeed, playerstats._toDefaultWalkSpeedLerpTime * Time.deltaTime);
        }

        //Run sidewards
        if (InputManager.CheckIfHorizontalInput())
        {
            playerstats._strafeSpeed = Mathf.Lerp(playerstats._strafeSpeed, playerstats._strafeRunSpeed, playerstats._toStrafeRunLerpTime * Time.deltaTime);
        }
        else
        {
            playerstats._strafeSpeed = Mathf.Lerp(playerstats._strafeSpeed, playerstats._defaultStrafeSpeed, playerstats._toDefaultStrafeSpeedLerpTime * Time.deltaTime);
        }

    }

    void TurnPlayer()
    {

        //Cast Ray from the middle of the screen
        Ray directionFromScreen = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        //Get the direction of the ray to slerp towards, so player will always believebly look forward

        //local varaible to hold the direction vector from screen center into world
        Vector3 direction = directionFromScreen.direction;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        //Spherical interpolate players y-rotation towards camera forward vector
        Quaternion from = transform.rotation;
        Quaternion to = lookRotation;
        float t = 0f;
        Quaternion lerpValue;

        t += playerstats._turnSpeed * Time.deltaTime;

        lerpValue = Quaternion.Slerp(transform.rotation, lookRotation, t);
        transform.rotation = lerpValue;

        //Zero out any rotations on x and z
        var zeroedOutEulers = transform.eulerAngles;
        zeroedOutEulers.x = 0;
        zeroedOutEulers.z = 0;
        transform.eulerAngles = zeroedOutEulers;

    }

    void CastRayFromFront()
    {
        RaycastHit hit;
        if (Physics.Raycast(castRayFrom.position, castRayFrom.forward, out hit, playerstats._LineOfSightDistance, interactionMaskRoom))
        {
            if (OnPlayerSeesSomethingInteractable_Room != null)
                OnPlayerSeesSomethingInteractable_Room.Invoke();
            Debug.Log("INVOKED SEEYING");
            isPlayerCanInteractBecauseHeLooksAtSmth_Room = true;
        }
        else if (Physics.Raycast(castRayFrom.position, castRayFrom.forward, out hit, playerstats._LineOfSightDistance, interactionMaskItem))
        {
            //OnPlayerSeesSomethingInteractable_Item.Invoke();
            isPlayerCanInteractBecauseHeLooksAtSmth_item = true;

        }
        else
        {
            if (OnPlayerDoesNotSeeSomehtingInteractable != null)
                OnPlayerDoesNotSeeSomehtingInteractable.Invoke();
            isPlayerCanInteractBecauseHeLooksAtSmth_Room = false;
            isPlayerCanInteractBecauseHeLooksAtSmth_item = false;
        }
        Debug.DrawRay(castRayFrom.position, castRayFrom.forward, Color.green);

    }

    void ResetPlayer()
    {
        {
            transform.position = resetTo.position;
        }
    }

    void AnimatePlayer()
    {
        playerAnimator.SetFloat("WalkSpeed", InputReceiver.movementInput.magnitude);
    }

    public static void SetPlayerToPlayableState()
    {
      
        isApplyGravity = true;
        InputReceiver.UnBlockMovementInputs();
    }
    public static void SetPlayerToUnplayableState()
    {
        isApplyGravity = false;
        InputReceiver.BlockMovementInput();
    }

    private void OnDisable()
    {
        InputReceiver.On_R_Input -= ResetPlayer;
        SceneLoader.OnSceneStartedLoading -= SetPlayerToUnplayableState;
        SceneLoader.OnScene_Has_Loaded -= SetPlayerToPlayableState;

    }
    private void OnEnable()
    {
        InputReceiver.On_R_Input += ResetPlayer;
        SceneLoader.OnSceneStartedLoading += SetPlayerToUnplayableState;
        SceneLoader.OnScene_Has_Loaded += SetPlayerToPlayableState;

    }
}
