using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    Transform target;

    // called once per frame after update
    void LateUpdate() {
        if(target) {
            transform.position = new Vector3(
                target.position.x, 
                target.position.y, 
                transform.position.z
            );
        }
    }

    public void SetTarget(Transform _target) {
        target = _target;
    }
}
