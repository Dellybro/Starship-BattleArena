using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] Vector2 moveSpeed = new Vector2(0.01f, 0.01f);
    
    Vector2 offset;
    Material material;

    void Awake() {
        material = GetComponent<SpriteRenderer>().material;    
    }

    // Update is called once per frame
    void Update()
    {
        offset = moveSpeed * Time.deltaTime;
        material.mainTextureOffset += offset;
    }
}
