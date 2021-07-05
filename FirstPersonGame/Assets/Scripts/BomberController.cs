using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BomberController : EnemyController
{
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerT = GameObject.Find("Player").transform;
    }

    void Update() {
        if (Vector3.Distance(transform.position, playerT.position) < MIN_DISTANCE_TO_PLAYER) {
            StartCoroutine(ExplosiveBehaviour());
        }
        else {
            agent.destination = playerT.position;
        }
    }

    private IEnumerator ExplosiveBehaviour() {
        agent.enabled = false;
        yield return new WaitForSeconds(2.5f);
        Vector3 exposivePos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(exposivePos, 3f);
        foreach(Collider collider in colliders) {
            if (!collider.CompareTag("Player")) {
                Destroy(collider.gameObject);
            }
            else {
                collider.gameObject.GetComponent<PlayerController>().health -= 50;
            }
        }
        GameObject.Find("LevelGenerator").GetComponent<RandomLevelGenerator>().BuildNavMeshOnLevel();
        Destroy(gameObject);
    }
}
