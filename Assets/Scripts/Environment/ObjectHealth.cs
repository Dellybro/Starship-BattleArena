using System;
using UnityEngine;
using Mirror;

public class ObjectHealth : NetworkBehaviour {
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] int maxHealth = 500;
    
    private int currentHealth;

    public static event Action<GameObject> ObjectOnDestroy;

    #region Server

    public override void OnStartServer() {
        currentHealth = maxHealth;
    }

    [Server]
    public void SetMaxHealth(int newMaxHealth) {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
    }

    [Server]
    public void ServerDamageDealt(int damage, int dealtBy) {
        if(currentHealth == 0) return;

        RpcLightUp();
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        if(currentHealth == 0) {
            ObjectOnDestroy?.Invoke(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }

    #endregion

    #region client

    [Command(requiresAuthority=false)]
    private void CmdDamageDealt(int damage, int dealtBy) {
        ServerDamageDealt(damage, dealtBy);
    }

    public void DamageDealt(int damage, int dealtBy) {
        CmdDamageDealt(damage, dealtBy);
    }

    [ClientRpc]
    private void RpcLightUp(){
        ParticleSystem instance = Instantiate(
            hitEffect, 
            transform.position, 
            Quaternion.identity);
        Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
    }

    #endregion
}
