using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlaceTower : NetworkBehaviour
{

    private GameObject towerPrefab;
    public GameObject towerTree;

    private Canvas canvas;

    [HideInInspector] public TowerTree activeBuildingTree;

    // Attack range of this tower
    public GameObject range;

    private UiManager _uiManager;
    private bool _clickDisabled;

    public GameObject TowerPrefab
    {
        get
        {
            return towerPrefab;
        }

        set
        {
            towerPrefab = value;
        }
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

    public bool ClickDisabled
    {
        get
        {
            return _clickDisabled;
        }

        set
        {
            _clickDisabled = value;
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening("UserClick", UserClick);
        EventManager.StartListening("DisableClick", DisableClick);
    }
    
    private void OnDisable()
    {
        EventManager.StopListening("UserClick", UserClick);
        EventManager.StopListening("DisableClick", DisableClick);
    }

    private void Awake()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canv in canvases)
        {
            if (canv.CompareTag("LevelUI"))
            {
                canvas = canv;
                break;
            }
        }
        ClickDisabled = false;
    }

    private void DisableClick(GameObject obj, string param)
    {
        ClickDisabled = true;
    }

    private void UserClick(GameObject obj, string param)
    {
        if (ClickDisabled)
            return;

        GameObject go = GameObject.Find("Defender");
        if (go == null)
            return;

        if (!go.GetComponent<PlayerHandler>().hasAuthority)
            return;

        ///////////////////////////////////////////////
        if (obj == gameObject) // This tower is clicked
        {
            if (activeBuildingTree == null)
            {
                //Show building tree
                activeBuildingTree = Instantiate<GameObject>(towerTree, canvas.transform).GetComponent<TowerTree>();
                activeBuildingTree.transform.position = Camera.main.WorldToScreenPoint(transform.position);
                activeBuildingTree.tower = this;
                //}
            }
        }
        else // Other click
        {
            // Close active building tree
            if (activeBuildingTree != null)
            {
                Destroy(activeBuildingTree.gameObject);
                return;
            }
        }


    }

    ///// ORIGINAL E FUNCIONANDO 
    public void Build(GameObject newTower)
    {
        if (activeBuildingTree != null)
            Destroy(activeBuildingTree.gameObject);

        //Command for spawn tower
        GameObject p = GameObject.Find("Defender");

        this.TowerPrefab = newTower;
        p.GetComponent<PlayerHandler>().Spawn(gameObject, newTower.name);
        Debug.Log("Tower Created");
    }


    ////////////////////////////////////////////////////////////////////////////////////////////
    ////// BELOW > TIREI O COMMAND PRA DEIXAR OS TESTES MAIS FACEIS
    public void Build2(GameObject newTower)
    {
        if (activeBuildingTree != null)
            Destroy(activeBuildingTree.gameObject);

        this.TowerPrefab = newTower;

        NetworkInstanceId towerId = gameObject.GetComponent<NetworkBehaviour>().netId;
        List<GameObject> prefabs = GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>().spawnPrefabs;
        GameObject newTowerPrefab = null;
        foreach (GameObject t in prefabs)
        {
            if (newTower.name == t.name)
            {
                newTowerPrefab = t;
                break;
            }
        }

        GameObject baseTower = NetworkServer.FindLocalObject(towerId);
        GameObject nTo = (GameObject)Instantiate(newTowerPrefab, baseTower.transform.position, baseTower.transform.rotation);
        NetworkServer.Spawn(nTo);
        NetworkServer.Destroy(baseTower);

        //////////////////////////////////////////////////////////////////////////////////////////
    }
}
