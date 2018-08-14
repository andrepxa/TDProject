using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHandler : NetworkBehaviour
{
    [SyncVar] private bool _ready;

    private UiManager _uiManager;

    [SyncVar] private string _placeNumber;

    [HideInInspector] [SyncVar(hook = "OnNameChange")] private string _name;

    [HideInInspector] [SyncVar(hook = "OnTagChange")] private string _tag;

    public bool Ready
    {
        get { return _ready; }
        set { _ready = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public string Tag
    {
        get { return _tag; }
        set { _tag = value; }
    }

    public UiManager UiManager
    {
        get
        {
            return _uiManager;
        }

        set
        {
            _uiManager = value;
        }
    }

    public string PlaceNumber
    {
        get
        {
            return _placeNumber;
        }

        set
        {
            _placeNumber = value;
        }
    }

    private void OnNameChange(string newName)
    {
        name = newName;
    }

    private void OnTagChange(string newTag)
    {
        tag = newTag;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnNameChange(Name);
        OnTagChange(Tag);
    }

    public override void OnStartLocalPlayer()
    {
        Text text = GameObject.Find("Text").GetComponent<Text>();

        if (gameObject.name == "Attacker")
        {
            GameObject bt = Resources.Load<GameObject>("Prefabs/Buttons");
            GameObject canvas = GameObject.Find("Canvas");
            Instantiate(bt, canvas.transform);

            text.text = "Attacker";
        }
        else if (gameObject.name == "Defender")
        {
            text.text = "Defender";
        }
    }

    [ClientRpc]
    public void RpcOnReconnect()
    {
        OnStartLocalPlayer();
        OnStartClient();
        Awake();
    }

    [Command]
    public void CmdRemoveOwnership(string role)
    {
        NetworkIdentity ni = GameObject.Find(role).GetComponent<NetworkIdentity>();
        ni.RemoveClientAuthority(ni.connectionToClient);
    }

    private void Update()
    {
        if (UiManager.Win)
        {
            Text text = GameObject.Find("Text").GetComponent<Text>();
            if (GetComponent<NetworkIdentity>().hasAuthority)
                if (gameObject.name == "Attacker")
                {
                    text.text = "Voce venceu!";
                }
                else if (gameObject.name == "Defender")
                {
                    text.text = "Voce perdeu!";
                }
        }
    }

    private void Awake()
    {
        UiManager = GameObject.Find("GameManager").GetComponent<UiManager>();
        ///// MENU CREATE FOR PLAYERS
        //if (gameObject.tag == "Defender")
        //{
        //    GameObject.Find("Canvas").GetComponent<CanvasManager>().DefenderButton();
        //}
        //else if (gameObject.tag == "Attacker")
        //{
        //    GameObject.Find("Canvas").GetComponent<CanvasManager>().AttackerButton();
        //}
    }

    [ClientRpc]
    public void RpcCreateButton()
    {
        GameObject.Find("Canvas").GetComponent<CanvasManager>().DefenderButton();
    }

    public void Spawn(GameObject currentTower, string towerPrefabName)
    {
        NetworkInstanceId towerId = currentTower.GetComponent<NetworkBehaviour>().netId;
        CmdSpawnTower(towerId, towerPrefabName);
        //CmdSpawnTower(baseTower);
    }

    [Command]
    void CmdSpawnTower(NetworkInstanceId towerId, string towerPrefabName)
    {
        //Dictionary<NetworkHash128, GameObject> prefabs = ClientScene.prefabs;
        List<GameObject> prefabs = GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>().spawnPrefabs;
        GameObject newTowerPrefab = null;
        foreach (GameObject t in prefabs)
        {
            if (towerPrefabName == t.name)
            {
                newTowerPrefab = t;
                break;
            }
        }

        GameObject baseTower = NetworkServer.FindLocalObject(towerId);
        GameObject newTower = (GameObject)Instantiate(newTowerPrefab, baseTower.transform.position, baseTower.transform.rotation);
        //GameObject newTower = (GameObject)Instantiate(baseTower.GetComponent<PlaceTower>().TowerPrefab, baseTower.transform.position, baseTower.transform.rotation);
        NetworkServer.Spawn(newTower);
        NetworkServer.Destroy(baseTower);
    }

    //[Command]
    //public void CmdReady(string tag)
    //{
    //    GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>().players[tag].Ready = true;
    //}

    [Command]
    public void CmdSpawnMob(string mobPrefabName)
    {
        //Dictionary<NetworkHash128, GameObject> prefabs = ClientScene.prefabs;
        List<GameObject> prefabs = GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>().spawnPrefabs;
        GameObject mob = null;
        foreach (GameObject t in prefabs)
        {
            if (mobPrefabName == t.name)
            {
                mob = t;
                break;
            }
        }

        //GameObject baseTower = NetworkServer.FindLocalObject(towerId);
        GameObject sp = GameObject.FindWithTag("Spawn");
        GameObject newMob = (GameObject)Instantiate(mob, sp.transform.position, sp.transform.rotation);
        newMob.GetComponent<AiStatePatrol>().path = GameObject.FindWithTag("Path").GetComponent<Pathway>();
        NavAgent agent = newMob.GetComponent<NavAgent>();

        float speedRandomizer = 0.2f;

        agent.Speed = Random.Range(agent.Speed * (1f - speedRandomizer), agent.Speed * (1f + speedRandomizer));
        NetworkServer.Spawn(newMob);
    }
}
