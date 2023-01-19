using UnityEngine;
using Mirror;

public class Shooter : NetworkBehaviour {
    [Header("General")]
    [SerializeField] GameObject firePosition;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float baseFiringRate = 0.5f;
    [SerializeField] float fireCoolDownTime;
    public int firePower;

    bool isFiring;
    Coroutine firingCoroutine;

    public bool isAI;

    void Update() {
        if(!hasAuthority) return;

        isFiring = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);
        Fire();
    }

    void Fire() {
        if(isFiring && Time.time > fireCoolDownTime) {
            fireCoolDownTime = Time.time + baseFiringRate;
            CmdFireWeapon();
        }
    }

    [Command]
    void CmdFireWeapon() {
        RpcFireWeapon(connectionToClient.connectionId, 10f, firePower);
    }

    #region Client

    [ClientRpc]
    void RpcFireWeapon(int _cid, float _speed, int _power) {
        GameObject instance = Instantiate(
            projectilePrefab, 
            firePosition.transform.position, 
            firePosition.transform.rotation
        );
        
        ShipControl controls = gameObject.GetComponent<ShipControl>();
        Projectile projectile = instance.GetComponent<Projectile>();
        projectile.Setup(GetComponent<Health>(), _cid, _speed, _power);
        instance.GetComponent<Rigidbody2D>().velocity = 
            instance.transform.up * (projectile.projectileSpeed + Mathf.Abs(controls.velocity.y));
        FindObjectOfType<SoundManager>().PlayShootSound(firePosition.transform.position);
    }

    #endregion
}
