using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AI_TheDoctor : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent agent;

    [SerializeField]
    private Transform target;

    private void Update()
    {
        agent.SetDestination(target.position);
    }
}
