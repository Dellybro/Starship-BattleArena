using System;
using System.Collections.Generic;
using Mirror;

public class Connection : NetworkManager {
    public List<ArenaPlayer> players {get;} = new List<ArenaPlayer>(); 

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    #region Server

    public override void OnServerAddPlayer(NetworkConnection conn) {
        base.OnServerAddPlayer(conn);

        ArenaPlayer player = conn.identity.GetComponent<ArenaPlayer>();
        players.Add(player);
    }

    public override void OnServerConnect(NetworkConnection conn) {
        if(numPlayers > maxConnections) {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        ArenaPlayer player = conn.identity.GetComponent<ArenaPlayer>();

        players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer() {
        players.Clear();
    }

    #endregion

    #region Client

    public override void OnStartClient() {
        base.OnStartClient();
    }

    public override void OnClientConnect(){
        base.OnClientConnect();
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(){
        base.OnClientDisconnect();
        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient() {
        players.Clear();
    }

    #endregion
}
