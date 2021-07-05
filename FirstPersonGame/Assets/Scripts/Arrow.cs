using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    void Start() {
        rb = GetComponent<Rigidbody>();
    }
    void Update() {
        if (!rb.isKinematic) {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }        
    }

    private void OnTriggerEnter(Collider col)
    {
        rb.isKinematic = true;
        Destroy(gameObject, 120f);
    }
}
