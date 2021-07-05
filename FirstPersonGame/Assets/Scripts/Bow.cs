using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    private const float SPEED_SCALE = 10f;
    [SerializeField]
    private Mesh stageOne, stageTwo;
    [SerializeField]
    private GameObject arrowPref;
    private GameObject currentArrow;
    private MeshFilter meshFilter;
    private PlayerController pController;
    [HideInInspector]
    public bool isDrawn = false;

    private void Awake() {
        meshFilter = GetComponent<MeshFilter>();
        pController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update() {
        if (isDrawn) {
            if (currentArrow == null) {
                meshFilter.mesh = stageTwo;
                currentArrow = Instantiate(arrowPref, transform);
            }
            if (Input.GetMouseButtonUp(1)) {
                Shoot();
            }
        }
    }

    private void Shoot() {
        pController.ModifyItemCount("Arrow");
        currentArrow.transform.parent = null;
        currentArrow.GetComponent<Rigidbody>().isKinematic = false;
        currentArrow.GetComponent<Rigidbody>().velocity = 
            currentArrow.transform.forward * SPEED_SCALE;
        meshFilter.mesh = stageOne;            
        isDrawn = false;
        currentArrow = null;        
    }
}
