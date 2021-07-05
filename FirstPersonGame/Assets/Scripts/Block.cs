using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int health { get; set; }
    [SerializeField]
    private BlockTypes blockType;

    private void Start() {
        health = (int)blockType;
    }

    public void DestroyBehaviour() {
        GameObject miniBlock = Resources.Load<GameObject>("mini" + blockType.ToString());
        Instantiate(miniBlock, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
