using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AISpawner : NetworkBehaviour {
    [SerializeField] float boundaryX = 40;
    [SerializeField] float boundaryY = 40;

    [SerializeField] GameObject enemyAIShip;
    [SerializeField] int maxShips = 6;
    
    List<GameObject> spawnedShips = new List<GameObject>();
    int nextShipId = 0;

    private void Update() {
        if(!isServer) return;

        for(int i = spawnedShips.Count - 1; i >= 0; i--) {
            if(spawnedShips[i] == null) spawnedShips.RemoveAt(i);
        }
        
        if(maxShips > spawnedShips.Count) {
            nextShipId ++;

            float rx = Random.Range(-boundaryX, boundaryX);
            float ry = Random.Range(-boundaryY, boundaryY);

            GameObject aiInstance = Instantiate(enemyAIShip, new Vector3(rx, ry, 0), Quaternion.identity);
            aiInstance.GetComponent<ShipDisplay>().SetDisplayName($"AI Ship #{nextShipId}");
            aiInstance.GetComponent<ShipDisplay>().SetArchetype(Helpers.Archetypes[Random.Range(0, Helpers.Archetypes.Count)]);
            spawnedShips.Add(aiInstance);
            NetworkServer.Spawn(aiInstance);

            if(nextShipId > 1000) nextShipId = 1;
        }
    }
}
