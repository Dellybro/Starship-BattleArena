using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("On trigger enter");
    }
}
