using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LookAtInterp : MonoBehaviour
{
    [SerializeField]
    MultiAimConstraint headTrack;
    [SerializeField]
    Transform headBoneTransform;
    [SerializeField]
    Transform targetLookAt;

    float newWeight;
    Vector3 dirToTarget;

    private void Update()
    {
        float curVel = headTrack.weight;

        dirToTarget = Vector3.Normalize(targetLookAt.position - headBoneTransform.position);
        
        newWeight = (Vector3.Dot(headBoneTransform.forward, dirToTarget));

        //if (newWeight > .25f || newWeight < -.25f)
            headTrack.weight = newWeight;
        //else
        //    headTrack.weight = 0;

        Debug.Log(newWeight);
    }
}
