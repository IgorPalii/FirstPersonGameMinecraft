using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    protected const float MIN_DISTANCE_TO_PLAYER = 1.5f;
    
    protected Transform playerT;
    protected NavMeshAgent agent;

    public int health { get; set; } = 500;
}
