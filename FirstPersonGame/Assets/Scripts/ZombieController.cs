using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : EnemyController
{
    private const float DELAY_ATTACK_TIME = 1.5f;
    private float prevAttackTime = 0f;
    
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        playerT = GameObject.Find("Player").transform;
    }

    void Update() {
        if (Vector3.Distance(transform.position, playerT.position) < MIN_DISTANCE_TO_PLAYER) {
            agent.enabled = false; //у скрипта бомбера нету этой строки
            AttackPlayer();
        }
        else {
            agent.enabled = true; //и этой
            agent.destination = playerT.position;
        }
    }

    private void AttackPlayer() {
        if (Time.time > prevAttackTime + DELAY_ATTACK_TIME) {
            playerT.gameObject.GetComponent<PlayerController>().health -= 15;
            prevAttackTime = Time.time;
            Debug.Log("Damaged");
        }
    }
}
