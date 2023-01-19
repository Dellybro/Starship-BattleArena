using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(CircleCollider2D))]
public class Blackhole : NetworkBehaviour {
    [SerializeField] Animator animator;
    [SerializeField] public float GRAVITY_PULL = 2500f;
    public static float m_GravityRadius = 1f;

    int removeAfter = 10;
    bool shrinkAndRemove = false;
    [SerializeField] public float movementX = 0.00f;
    [SerializeField] public float movementY = 0.00f;

    public override void OnStartServer() {
        if(!isServer) return;
        StartCoroutine(Deactivate());
    }

    IEnumerator Deactivate() {
        yield return new WaitForSeconds(removeAfter);
        shrinkAndRemove = true;
    }

    void Awake() {
        if(!isServer) return;

        m_GravityRadius = GetComponent<CircleCollider2D>().radius;
    }

    private void Update() {
        if(!isServer) return;

        transform.Translate(new Vector3(movementX,movementY,0));

        if(shrinkAndRemove) {
            transform.localScale += new Vector3(-0.001f, -0.001f, 0f);
            if(transform.localScale.x <= 0 || transform.localScale.x <= 0) {
                NetworkServer.Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.attachedRigidbody) {
            other.attachedRigidbody.AddForce(new Vector2(0,0));
            other.attachedRigidbody.velocity = Vector3.zero;
            other.attachedRigidbody.angularVelocity = 0f;
        }
    }

     /// <summary>
     /// Attract objects towards an area when they come within the bounds of a collider.
     /// This function is on the physics timer so it won't necessarily run every frame.
     /// </summary>
     /// <param name="other">Any object within reach of gravity's collider</param>
    void OnTriggerStay2D(Collider2D other) {
        if (other.attachedRigidbody) {
            float gravityIntensity = Vector3.Distance(transform.position, other.transform.position) / m_GravityRadius;
            other.attachedRigidbody.AddForce((transform.position - other.transform.position) * gravityIntensity * other.attachedRigidbody.mass * GRAVITY_PULL * Time.smoothDeltaTime);
            Debug.DrawRay(other.transform.position, transform.position - other.transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.attachedRigidbody) {
            other.attachedRigidbody.AddForce(new Vector2(0,0));
            other.attachedRigidbody.velocity = Vector3.zero;
            other.attachedRigidbody.angularVelocity = 0f;
        }
    }
}
