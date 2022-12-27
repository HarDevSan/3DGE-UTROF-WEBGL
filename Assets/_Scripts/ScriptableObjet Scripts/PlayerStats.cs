using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
public class PlayerStats : ScriptableObject
{
    //default values
    public float _defaultWalkSpeed;
    public float _defaultStrafeSpeed;

    //Movement related variables
    public float _walkSpeed;
    public float _runSpeed;
    public float _walkBackwardsSpeed;
    public float _turnSpeed;
    public float _strafeSpeed;
    public float _strafeRunSpeed;
    public float _gravityStrength;
    public float _toRunLerpTime;
    public float _toStrafeRunLerpTime;
    public float _LineOfSightDistance;
    public float _LineToStepDetectDistance;


    //ease in to default variables
    public float _toDefaultWalkSpeedLerpTime;
    public float _toDefaultStrafeSpeedLerpTime;



}

