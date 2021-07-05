using System.Collections;
using System;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemyes;
    private EnemyTypes spawnerType;
    private float timeBetweenSpawn = 5f;

    void Start()
    {
        spawnerType = (EnemyTypes)UnityEngine.Random.Range(0, Enum.GetNames(typeof(EnemyTypes)).Length);
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(5f);
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawn);
            Instantiate(enemyes[(int)spawnerType], transform.position, Quaternion.identity);
        }       
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("Enemy"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.position = new Vector3(transform.position.x, col.transform.position.y + 1, transform.position.z);
        }        
    }
}
