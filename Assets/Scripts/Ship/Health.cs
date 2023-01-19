using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;

public class Health : NetworkBehaviour
{

    [SerializeField] ParticleSystem hitEffect;

    [SyncVar(hook=nameof(HandleMaxHealthUpdated))]
    [SerializeField] int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;

    // public static event Action<PlayerController> PlayerGotHit;
    public event Action<int, int> ClientOnHealthUpdated;

    public static event Action<Health, string> ServerOnDie;

    public bool isAi;
    public List<int> damageLog = new List<int>();

    #region server

    public override void OnStartServer() {
        currentHealth = maxHealth;
    }

    [Server]
    public void SetMaxHealth(int newMaxHealth) {
        maxHealth = newMaxHealth;
    }

    [Server]
    public void Heal(int healAmount) {
        if(!isServer) return;

        currentHealth += healAmount;
        if(currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }

        if(!isAi) TargetRpcHealthUpdated(currentHealth, maxHealth, 1);
    }

    [Server]
    public void ServerDamageDealt(int damage, int dealtBy, int projectileId) {
        if(currentHealth == 0) return;

        if(isAi) {
            if(!damageLog.Contains(projectileId)) {
                damageLog.Add(projectileId);
                currentHealth = Mathf.Max(currentHealth - damage, 0);
                RpcLightUp();

                // Target player if player is shooter.
                if(NetworkServer.connections.ContainsKey(dealtBy)) {
                    if(NetworkServer.connections[dealtBy].identity.TryGetComponent<ArenaPlayer>(out ArenaPlayer _player)) {
                        GetComponent<EnemyAI>().target = _player.GetShip();
                    }
                }

            }
        }
        else {
            currentHealth = Mathf.Max(currentHealth - damage, 0);
            TargetRpcHealthUpdated(currentHealth, maxHealth, 2);
            RpcLightUp();
        }

        if(currentHealth == 0) {
            string killerName;
            if(!isAi) {
                ArenaPlayer killed = connectionToClient.identity.GetComponent<ArenaPlayer>();
                killed.ServerIncrementDeaths();
                killerName = killed.shipData.name;
            }
            else {
                killerName = EnemyAI.AI_KILLER_IDENTIFIER_STRING;
            }

            if(dealtBy == Floater.FLOATER_IDENTIFIER) {
                RpcSendDeathLog(killerName, Floater.FLOATER_IDENTIFIER_STRING);
                ServerOnDie?.Invoke(this, Floater.FLOATER_IDENTIFIER_STRING);
            }
            else if(dealtBy == EnemyAI.AI_IDENTIFIER) {
                RpcSendDeathLog(killerName, EnemyAI.AI_IDENTIFIER_STRING);
                ServerOnDie?.Invoke(this, EnemyAI.AI_IDENTIFIER_STRING);
            }
            else {
                ArenaPlayer killer = NetworkServer.connections[dealtBy].identity.GetComponent<ArenaPlayer>();
                killer.ServerIncrementKills();
                
                RpcSendDeathLog(killerName, killer.shipData.name);
                ServerOnDie?.Invoke(this, killer.shipData.name);
            }
            
            if(!isAi) {
                ArenaPlayer killed = connectionToClient.identity.GetComponent<ArenaPlayer>();
                killed.ServerIncrementDeaths();
            }

            NetworkServer.Destroy(gameObject);
        }

    }

    [Command]
    private void CmdDamageDealt(int damage, int dealtBy, int projectileId) {
        ServerDamageDealt(damage, dealtBy, projectileId);
    }

    public void DamageDealt(int damage, int dealtBy, int projectileId) {
        if(!hasAuthority) return;

        CmdDamageDealt(damage, dealtBy, projectileId);
    }

    [Command(requiresAuthority=false)]
    private void CmdDamageDealtAI(int damage, int dealtBy, int projectileId) {
        ServerDamageDealt(damage, dealtBy, projectileId);
    }

    public void DamageDealtAI(int damage, int dealtBy, int projectileId) {
        CmdDamageDealtAI(damage, dealtBy, projectileId);
    }

    #endregion

    #region client

    private void HandleMaxHealthUpdated(int oldMax, int newMax) {
        currentHealth = newMax;
    }

    private void HandleHealthUpdated(int oldHealth, int newHealth) {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    public int GetCurrentHealth() {
        return currentHealth;
    }

    public int GetMaxHealth() {
        return maxHealth;
    }

    [ClientRpc]
    private void RpcLightUp(){
        ParticleSystem instance = Instantiate(
            hitEffect, 
            transform.position, 
            Quaternion.identity);
        Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
    }

    [ClientRpc]
    private void RpcSendDeathLog(string killed, string killer) {
        FindObjectOfType<EventLogs>().AddKillLog(killed, killer);
    }

    [TargetRpc]
    public void TargetRpcSendHealth() {
        FindObjectOfType<MainHUD>().HandleHealthUpdated(currentHealth, maxHealth);
    }
    
    [TargetRpc]
    public void TargetRpcHealthUpdated(int newHealth, int maxHealth, int sound) {
        FindObjectOfType<MainHUD>().HandleHealthUpdated(newHealth, maxHealth);
        if(sound == 1) {
            FindObjectOfType<SoundManager>().PlayHealSound(transform.position);
        }
        else if(sound == 2) {
            FindObjectOfType<SoundManager>().PlayImpactSound(transform.position, SoundManager.ImpactSoundType.Ship);
        }
    }

    #endregion
}
