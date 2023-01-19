using System;
using System.Collections;
using UnityEngine;
using Mirror;

public class HealthPack : NetworkBehaviour {
    [SerializeField] [Range(0,1)] float healthPercentage = 0.25f;
    [SerializeField] int removeAfter = 2 * 60; // 2mins
    
    public static event Action ServerHealthPackUsed;

    public override void OnStartServer() {
        if(!isServer) return;

        StartCoroutine(Deactivate());
    }

    IEnumerator Deactivate() {
        yield return new WaitForSeconds(removeAfter);
        NetworkServer.Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(!isServer) return;

        if(other.TryGetComponent<Health>(out Health health)){
            health.Heal(Mathf.FloorToInt(health.GetMaxHealth() * healthPercentage));
            NetworkServer.Destroy(gameObject);
            ServerHealthPackUsed?.Invoke();
        }
    }
}
