using UnityEngine;
using Mirror;

public class PackSpawners : NetworkBehaviour {
    [SerializeField] float boundaryX = 40;
    [SerializeField] float boundaryY = 40;

    [SerializeField] GameObject healthPackPrefab;
    double timeBetweenPacks = 20;
    double nextSpawnedPackAt;

    public override void OnStartServer() {
        HealthPack.ServerHealthPackUsed += HandleHealthPackUsed;
    }

    public override void OnStopServer() {
        HealthPack.ServerHealthPackUsed -= HandleHealthPackUsed;
    }

    public void HandleHealthPackUsed() {
        nextSpawnedPackAt = NetworkTime.time;
    }

    private void Update() {
        if(!isServer) return;
        
        if(nextSpawnedPackAt < NetworkTime.time) {
            nextSpawnedPackAt = NetworkTime.time + timeBetweenPacks;


            float rx = Random.Range(-boundaryX, boundaryX);
            float ry = Random.Range(-boundaryY, boundaryY);

            GameObject pInstance = Instantiate(healthPackPrefab, new Vector3(rx, ry, 0), Quaternion.identity);
            NetworkServer.Spawn(pInstance);
        }
    }
}
