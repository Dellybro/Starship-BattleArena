using UnityEngine;
using Mirror;

public class BlackholeSpawner : NetworkBehaviour {
    [SerializeField] float boundaryX = 20;
    [SerializeField] float boundaryY = 20;
    [SerializeField] float speedX = 0.05f;
    [SerializeField] float speedY = 0.05f;

    [SerializeField] GameObject blackHolePrefab;
    double timeBetweenBlackHoles = 5;
    double lastSpawnedBlackHole;

    [SerializeField] bool isActive;

    private void Update() {
        if(!isServer) return;
        if(!isActive) return;

        if(lastSpawnedBlackHole < NetworkTime.time) {
            lastSpawnedBlackHole = NetworkTime.time + timeBetweenBlackHoles;


            float rx = Random.Range(-boundaryX, boundaryX);
            float ry = Random.Range(-boundaryY, boundaryY);

            GameObject pInstance = Instantiate(blackHolePrefab, new Vector3(rx, ry, 0), Quaternion.identity);
            pInstance.GetComponent<Blackhole>().movementX = Random.Range(-speedX, speedX);
            pInstance.GetComponent<Blackhole>().movementY = Random.Range(-speedY, speedY);
            NetworkServer.Spawn(pInstance);
        }
    }
}
