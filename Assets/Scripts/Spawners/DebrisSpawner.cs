using Mirror;
using UnityEngine;

public class DebrisSpawner : NetworkBehaviour
{
    [Header("Boundary & Speed")]
    [SerializeField] float boundaryX = 40;
    [SerializeField] float boundaryY = 40;
    [SerializeField] float speedX = 0.03f;
    [SerializeField] float speedY = 0.03f;

    [Header("Debris")]
    [SerializeField] GameObject debrisPrefab;
    [SerializeField] int maxDebris = 7;
    int numberOfDebris = 0;
    
    [Header("Asteroid")]
    [SerializeField] GameObject astroidPrefab;
    [SerializeField] int maxAsteroid = 7;
    int numberOfAsteroids = 0;

    float spfa = 5;

    void Start() {
        ObjectHealth.ObjectOnDestroy += HandleObjectDestroy;
    }

    void OnDestroy() {
        ObjectHealth.ObjectOnDestroy -= HandleObjectDestroy;
    }

    void HandleObjectDestroy(GameObject obj) {
        if(obj.TryGetComponent<Floater>(out Floater floater)) {
            if(floater.typeOf == "Asteroid") numberOfAsteroids--;
            if(floater.typeOf == "Debris") numberOfDebris--;
        }
    }

    private void Update() {
        if(!isServer) return;
        
        if(spfa > 0) {
            spfa -= Time.deltaTime;
            return;
        }

        // Asteroids
        if(numberOfAsteroids < maxAsteroid) {
            numberOfAsteroids++;
            float rx = Random.Range(-boundaryX, boundaryX);
            float ry = Random.Range(-boundaryY, boundaryY);

            GameObject aInstance = Instantiate(astroidPrefab, new Vector3(rx, ry, 0), Quaternion.identity);
            aInstance.GetComponent<Floater>().typeOf = "Asteroid";
            aInstance.GetComponent<Floater>().movementX = Random.Range(-speedX, speedX);
            aInstance.GetComponent<Floater>().movementY = Random.Range(-speedY, speedY);
            NetworkServer.Spawn(aInstance);
        }

        // Debris
        if(numberOfDebris < maxDebris) {
            numberOfDebris++;
            float rx = Random.Range(-boundaryX, boundaryX);
            float ry = Random.Range(-boundaryY, boundaryY);

            GameObject pInstance = Instantiate(debrisPrefab, new Vector3(rx, ry, 0), Quaternion.identity);
            pInstance.GetComponent<Floater>().typeOf = "Debris";
            pInstance.GetComponent<Floater>().movementX = Random.Range(-speedX, speedX);
            pInstance.GetComponent<Floater>().movementY = Random.Range(-speedY, speedY);
            NetworkServer.Spawn(pInstance);
        }
    }
}
