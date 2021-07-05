using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    private const int PIRAMID_COUNT = 5;
    private const int PIRAMID_SIZE = 11;
    private const int PIRAMID_HEIGHT = PIRAMID_SIZE / 2 + 1;
    [SerializeField]
    private GameObject groundCube;
    void Start() {
        for (int i = 0; i < PIRAMID_COUNT; i++) {
            for (int j = 0; j < PIRAMID_COUNT; j++) {
                CreatePyramide(new Vector3(i * PIRAMID_SIZE, 0, j * PIRAMID_SIZE));
            }
        }
    }

    void CreatePyramide(Vector3 startPos) {
        int offsetX = 0;
        int offsetZ = 0;
        for (int i = 0; i < PIRAMID_HEIGHT; i++) { 
            for (int j = (int)startPos.x + offsetX; j < (int)startPos.x + PIRAMID_SIZE - offsetX; j++) {
                for (int k = (int)startPos.z + offsetZ; k < startPos.z + PIRAMID_SIZE - offsetZ; k++) { 
                    Instantiate(groundCube, new Vector3(j, i, k), Quaternion.identity);
                }
            }
            offsetZ++;
            offsetX++;
        }
    }
}
