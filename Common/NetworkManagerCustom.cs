using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;


public class NetworkManagerCustom : NetworkManager {

    [HideInInspector] private Dictionary<string, string> _connections = new Dictionary<string, string>();

    private IEnumerator coroutine;

    public Dictionary<string, string> Connections
    {
        get
        {
            return _connections;
        }

        set
        {
            _connections = value;
        }
    }

    // Client
    public override void OnClientConnect(NetworkConnection conn)
    {
        // A custom identifier we want to transmit from client to server on connection
        string id = GetCustomID();

        StringMessage msg = new StringMessage(id);

        // Create message which stores our custom identifier
        if (!clientLoadedScene)
        {
            // Call Add player and pass the message
            ClientScene.AddPlayer(conn, 0, msg);
        }
    }

    private string GetCustomID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        // Create message which stores our custom identifier
        string id = GetCustomID();

        StringMessage msg = new StringMessage(id);

        // always become ready.
        ClientScene.Ready(conn);

        bool addPlayer = (ClientScene.localPlayers.Count == 0);
        bool foundPlayer = false;
        for (int i = 0; i < ClientScene.localPlayers.Count; i++)
        {
            if (ClientScene.localPlayers[i].gameObject != null)
            {
                foundPlayer = true;
                break;
            }
        }
        if (!foundPlayer)
        {
            // there are players, but their game objects have all been deleted
            addPlayer = true;
        }
        if (addPlayer)
        {
            // Call Add player and pass the message
            ClientScene.AddPlayer(conn, 0, msg);
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader msgReader)
    {
        GameObject player = (GameObject)GameObject.Instantiate(playerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        PlayerHandler ph = player.GetComponent<PlayerHandler>();
        string lastRole;
        string id = null;

        if (msgReader != null)
        {
            StringMessage i = msgReader.ReadMessage<StringMessage>();
            id = i.value;
        }

        if (Connections.ContainsKey(id))
        {
            Connections.TryGetValue(id, out lastRole);
            
            GameObject p = GameObject.FindWithTag(lastRole);
            if (p != null)
            {
                NetworkServer.ReplacePlayerForConnection(conn, p, playerControllerId);
                p.GetComponent<NetworkIdentity>().RemoveClientAuthority(p.GetComponent<NetworkIdentity>().connectionToClient);
                p.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
                ph.RpcOnReconnect();
                coroutine = WaitAndCreate(conn, playerControllerId, p, ph, id, lastRole);
                StartCoroutine(coroutine);
            } else
            {
                DefineRole(lastRole, id, ph);
                NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            }
        }
        else
        {
            GameObject p = GameObject.FindWithTag("Attacker");
            if (p == null)
            {
                DefineRole("Attacker", id, ph);
            }
            else
            {
                DefineRole("Defender", id, ph);
            }

            Connections.Add(id, player.tag);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
    }

    IEnumerator WaitAndCreate(NetworkConnection conn, short playerControllerId, GameObject p, PlayerHandler ph, string id, string lastRole)
    {
        // suspend execution for 5 seconds
        while (p != null)
        {
            p = GameObject.Find(lastRole);
            yield return new WaitForSeconds(0.5f);
        }
        DefineRole(lastRole, id, ph);
        NetworkServer.AddPlayerForConnection(conn, ph.gameObject, playerControllerId);
        yield break;
    }

    private void DefineRole(string role, string id, PlayerHandler handler)
    {
        handler.PlaceNumber = id;
        handler.Name = handler.Tag = handler.gameObject.name = handler.gameObject.tag = role;
    }

}
