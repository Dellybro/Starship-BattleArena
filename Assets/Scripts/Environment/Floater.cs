using System.Collections;
using UnityEngine;
using Mirror;

public class Floater : NetworkBehaviour {
    static public int FLOATER_IDENTIFIER = -2834049;
    static public string FLOATER_IDENTIFIER_STRING = "a floating object!";

    [Header("Sprite and RG2B")]
    [SerializeField] Sprite[] variations;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] public float movementX = 0.00f;
    [SerializeField] public float movementY = 0.00f;
    float rotationSpeed = 0.35f;

    [Header("Environment")]
    public int power = 50;
    public string typeOf;

    // Start is called before the first frame update
    void Start() {
        if(!isServer) return;

        spriteRenderer.sprite = variations[Random.Range(0, variations.Length)];
    }

    void OnCollisionEnter2D(Collision2D other) {
        if(!isServer) return;

        if(other.collider.TryGetComponent<Health>(out Health health)){
            health.ServerDamageDealt(power, Floater.FLOATER_IDENTIFIER, Floater.FLOATER_IDENTIFIER);
            gameObject.GetComponent<ObjectHealth>().ServerDamageDealt(power, -1);
        }
    }

    // Update is called once per frame
    void Update() {
        if(!isServer) return;

        transform.Translate(new Vector3(movementX,movementY,0));
        transform.Rotate(new Vector3(0,0,rotationSpeed));
    }
}
