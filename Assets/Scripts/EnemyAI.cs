using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemyAI : NetworkBehaviour {
    static public string AI_KILLER_IDENTIFIER_STRING = "a stupid AI ship!";
    static public string AI_IDENTIFIER_STRING = "a stupid AI ship!";
    static public int AI_IDENTIFIER = -2834069;
    public GameObject target;

    [Header("Attack")]
    [SerializeField] float weaponSpeed = 10f;
    [SerializeField] int damage = 25;
    [SerializeField] float baseFiringRate = 0.5f;
    [SerializeField] float fireCoolDownTime;
    [SerializeField] GameObject firePosition;
    [SerializeField] GameObject projectilePrefab;

    [Header("Range")]
    [SerializeField] float nearbyRadius = 100f;
    [SerializeField] float range = 18f;

    private float accel = 10f;
    private float maxAccel = 25f;
    private float velocityDrag = 1f; // how fast the ship slows down
    private Vector3 velocity; // movement mostly in the Y direction
    private float zRotationVelocity; // movement in the Z direciton

    void Update() {
        if(!isServer) return;
        if(target == null) target = null;

        Health health = GetComponent<Health>();

        if(!target) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, nearbyRadius);
            foreach(Collider2D collider in colliders) {
                if(collider.TryGetComponent<Health>(out Health _target)) {
                    if(_target.gameObject == gameObject) return;

                    target = _target.gameObject;
                }
            }
        }
        else if(target) {
            MoveToTargetUpdate();
            AttackTarget();
        }
    }

    void FixedUpdate() {
        if(!isServer) return;
        if(!target) return;

        MoveToTargetFixedUpdate();
    }

    [Server]
    void AttackTarget() {
        float distance = Vector3.Distance(target.transform.position, transform.position);
        if(distance > range) return;

        if(Time.time > fireCoolDownTime) {
            fireCoolDownTime = Time.time + baseFiringRate;
            RpcFireWeapon(EnemyAI.AI_IDENTIFIER, weaponSpeed, damage);
        }
    }

    [ClientRpc]
    void RpcFireWeapon(int _cid, float _speed, int _power) {
        GameObject instance = Instantiate(
            projectilePrefab, 
            firePosition.transform.position, 
            firePosition.transform.rotation
        );

        Projectile projectile = instance.GetComponent<Projectile>();
        projectile.Setup(GetComponent<Health>(), _cid, _speed, _power);
        instance.GetComponent<Rigidbody2D>().velocity = 
            instance.transform.up * (projectile.projectileSpeed + Mathf.Abs(velocity.y));
    }

    void MoveToTargetUpdate() {
        float distance = Vector3.Distance(target.transform.position, transform.position);
        
        if(distance < range) {
            // Orbit
            Vector3 acceleration = accel * transform.right;
            velocity += (acceleration / 15f) * Time.deltaTime;
            
        } else {
            // Set velocity if there is a target.
            Vector3 acceleration = accel * transform.up;
            velocity += acceleration * Time.deltaTime;
        }

    }

    void MoveToTargetFixedUpdate() {
        // apply velocity drag
        velocity = velocity * (1 - Time.deltaTime * velocityDrag);

        // clamp to maxAccel
        velocity = Vector3.ClampMagnitude(velocity, maxAccel);

        // update transform
        transform.position += velocity * Time.deltaTime;

        transform.up = target.transform.position - transform.position;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, nearbyRadius);
    }
}
