using System;
using System.Collections;
using UnityEngine;
using Mirror;

public class ArenaPlayer : NetworkBehaviour
{   
    [SerializeField] GameObject shipPrefab;

    [SyncVar(hook = nameof(HandleKillsUpdated))]
    public int kills;
    
    [SyncVar]
    public int deaths;

    [SyncVar(hook = nameof(ClientHandleShipDataUpdated))]
    public ShipModel shipData;
    private ShipModel nextShip;

    private GameObject ship = null;
    public static Action ClientOnInfoUpdated;
    public static Action ShipOnUpdated;
    public static Action ClientKillsUpdated;


    void Start() {
        if(!hasAuthority) return;
    }

    public GameObject GetShip() {
        return ship;
    }

    
    [Server]
    IEnumerator Respawn(float timeToWait) {
        yield return new WaitForSeconds(timeToWait);

        ship = Instantiate(
            shipPrefab,
            NetworkManager.singleton.GetStartPosition().position,
            Quaternion.identity
        );
        TargetRpcShipRespawned(shipData, ship.transform.position);

        ship.GetComponent<Shooter>().firePower = shipData.FirePower();
        ship.GetComponent<ShipControl>().SetSteer(shipData.SteerPower());
        ship.GetComponent<ShipControl>().SetAccel(shipData.Acceleration());
        ship.GetComponent<Health>().SetMaxHealth(shipData.GetMaxHealth());
        ship.GetComponent<ShipDisplay>().SetDisplayName(shipData.name);
        ship.GetComponent<ShipDisplay>().SetArchetype(shipData.archetype);
        NetworkServer.Spawn(ship, connectionToClient);

        // Not really sure why this doesn't work on the health.OnStartServer()?
        ship.GetComponent<Health>().TargetRpcSendHealth();
    }


    #region Server

    public override void OnStartServer() {
        Health.ServerOnDie += ServerHandleShipDied;

        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer() {
        Health.ServerOnDie -= ServerHandleShipDied;
    }

    private void ServerHandleShipDied(Health health, string killedBy) {
        if(health.isAi) return;
        if(health.connectionToClient.connectionId != connectionToClient.connectionId) return;

        if(nextShip != null) {
            SetShipData(nextShip);
            SetNextShipData(null);
        }

        ship = null;
        float shipRespawnTime = 5 + deaths % 5;

        TargetRpcSetRespawnTimer(shipRespawnTime,killedBy);
        StartCoroutine(Respawn(shipRespawnTime));
    }

    [Server]
    public void SetShipData(ShipModel data) {
        shipData = data;
        if(!ship) StartCoroutine(Respawn(0));
    }

    [Command]
    public void CmdSetShipData(ShipModel data) {
        SetShipData(data);
    }


    [Server]
    public void SetNextShipData(ShipModel data) {
        nextShip = data;
    }

    [Command]
    public void CmdSetNextShipData(ShipModel data) {
        SetNextShipData(data);
    }

    public void SetNextShip(ShipModel data) {
        if(!hasAuthority) return;

        if(shipData == null) {
            CmdSetShipData(data);
        }
        else CmdSetNextShipData(data);
    }

    /** Kills */
    [Server]
    public void ServerIncrementKills() {
        kills += 1;
        TargetRpcKillsUpdated(kills);
    }

    [Command]
    private void CmdIncrementKills() {
        ServerIncrementKills();
    }

    public void IncrementKills() {
        if(!hasAuthority) return;
        
        CmdIncrementKills();
    }

    /** Deaths */
    [Server]
    public void ServerIncrementDeaths() {
        deaths += 1;
        TargetRpcDeathsUpdated(deaths);
    }

    [Command]
    private void CmdIncrementDeaths() {
        ServerIncrementDeaths();
    }

    public void IncrementDeaths() {
        if(!hasAuthority) return;
        
        CmdIncrementDeaths();
    }

    #endregion

    #region Client

    public override void OnStartClient() {
        if (!isClientOnly) { return; }

        DontDestroyOnLoad(gameObject);
        ((Connection)NetworkManager.singleton).players.Add(this);
    }

    private void ClientHandleShipDataUpdated(ShipModel oldShip, ShipModel newShip) {
        ClientOnInfoUpdated?.Invoke();
    }

    public override void OnStopClient() {
        if (!isClientOnly) { return; }
        
        ((Connection)NetworkManager.singleton).players.Remove(this);
    }


    private void HandleShipUpdated(Color oldColor, Color newColor) {
        ShipOnUpdated?.Invoke();
    }

    private void HandleKillsUpdated(int oldKills, int newKills) {
        ClientKillsUpdated?.Invoke();
    }

    [TargetRpc]
    private void TargetRpcKillsUpdated(int killCount) {
        FindObjectOfType<MainHUD>().HandleKillCountUpdated(killCount);
    }

    [TargetRpc]
    private void TargetRpcDeathsUpdated(int deathCount) {
        FindObjectOfType<MainHUD>().HandleDeathCountUpdated(deathCount);
    }

    [TargetRpc]
    private void TargetRpcSetRespawnTimer(double time, string killedBy) {
        FindObjectOfType<RespawnTimer>().SetRespawnTime(time, killedBy);
    }

    [TargetRpc]
    private void TargetRpcShipRespawned(ShipModel shipData, Vector3 position) {
        FindObjectOfType<MainHUD>().HandleShipSpawned(shipData);
        FindObjectOfType<SoundManager>().PlayRespawnSound(position);
    }

    #endregion
}
