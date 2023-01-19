using System.Collections.Generic;
using Mirror;
using UnityEngine;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] GameObject[] leaders = new GameObject[6];
    ArenaPlayer myPlayer;

    void Start() {
        ArenaPlayer.ClientKillsUpdated += HandleClientInfoUpdated;
        ArenaPlayer.ClientOnInfoUpdated += HandleClientInfoUpdated;
        Connection.ClientOnConnected += HandleClientInfoUpdated;
        Connection.ClientOnDisconnected += HandleClientInfoUpdated;

        HandleClientInfoUpdated();
    }

    void OnDestroy() {
        ArenaPlayer.ClientKillsUpdated += HandleClientInfoUpdated;
        ArenaPlayer.ClientOnInfoUpdated += HandleClientInfoUpdated;
        Connection.ClientOnConnected += HandleClientInfoUpdated;
        Connection.ClientOnDisconnected += HandleClientInfoUpdated;
    }

    private void HandleClientInfoUpdated() {
        List<ArenaPlayer> players = ((Connection)NetworkManager.singleton).players;

        players.Sort(delegate(ArenaPlayer x, ArenaPlayer y) {
            if(x.kills > y.kills) return -1;
            if(x.kills < y.kills) return 1;
            return 0;
        });


        if(NetworkClient.active && !myPlayer) {
            myPlayer = NetworkClient.connection?.identity?.GetComponent<ArenaPlayer>();
        }

        for(int i = 0; i < leaders.Length; i++) {
            if(players.Count > i) {
                leaders[i].gameObject.SetActive(true);
                TMP_Text name = leaders[i].transform.GetChild(0).GetComponent<TMP_Text>();
                TMP_Text score = leaders[i].transform.GetChild(1).GetComponent<TMP_Text>();
                if(players[i].shipData != null) {
                    if(players[i].shipData.name.Length > 13) {
                        name.text = $"{players[i].shipData.name.Substring(0, 12)}...";
                    } else name.text = $"{players[i].shipData.name}";
                } else name.text = "New Player";

                score.text = $"{players[i].kills}";
                
                if(NetworkClient.active && players[i].netId == myPlayer?.netId) {
                    FindObjectOfType<MainHUD>().HandleRankUpdated(i+1);
                }
            }
            else {
                leaders[i].gameObject.SetActive(false);
            }
        }
    }
}
