using System;
using UnityEngine;
using Mirror;

public class ShipControl : NetworkBehaviour 
{   
    [SerializeField] Health health;
    [SerializeField] Rigidbody2D rg2b; // The rigid body which will be DESTROYED
    ArenaPlayer player;

    private float steer = 250f;
    private float accel = 10f;
    private float maxAccel = 25f;
    private float maxSteer = 400f;

    public float velocityDrag = 1f; // how fast the ship slows down
    public float rotationDrag = 1f; // how fast the rotation slows down

    public Vector3 velocity; // movement mostly in the Y direction
    private float zRotationVelocity; // movement in the Z direciton

    private Vector2 movement; // Player input

    void Start() {
        if(!hasAuthority) return;
        
        Camera.main.GetComponent<FollowPlayer>().SetTarget(transform);
        player = NetworkClient.connection.identity.GetComponent<ArenaPlayer>();
    }

    public void SetSteer(float _steer) {
        steer = Math.Min(_steer, maxSteer);
    }
    public void SetAccel(float _accel) {
        accel = Math.Min(_accel, maxAccel);
    }

    // Update is called once per frame
    void Update() {
        if(!hasAuthority) return;

        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if(movement.y == 0 && movement.x == 0) {
            // Turn off engines
        }

        // Get the upward position of the transform and apply user input y * acceleration
        Vector3 acceleration = movement.y * accel * transform.up;
        velocity += acceleration * Time.deltaTime;

        // Apply steer we use a -1 here so that right turns clockwise, left counterclock
        float zTurnAcceleration = -1 * movement.x * steer;
        zRotationVelocity += zTurnAcceleration * Time.deltaTime;
    }

    private void FixedUpdate() {
        if(!hasAuthority) return;

        // After the update we want to lower the velocity being applied

        // apply velocity drag
        velocity = velocity * (1 - Time.deltaTime * velocityDrag);

        // clamp to maxAccel
        velocity = Vector3.ClampMagnitude(velocity, maxAccel);

        // apply rotation drag
        zRotationVelocity = zRotationVelocity * (1 - Time.deltaTime * rotationDrag);

        // clamp to maxSteer
        zRotationVelocity = Mathf.Clamp(zRotationVelocity, -maxSteer, maxSteer);

        // update transform
        transform.position += velocity * Time.deltaTime;
        transform.Rotate(0, 0, zRotationVelocity * Time.deltaTime);
    }

    #region server

    #endregion

    #region client

    #endregion
}
