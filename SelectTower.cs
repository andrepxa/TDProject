using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTower : MonoBehaviour
{

    public GameObject towerPrefab;
    private TowerTree myTree;

    private void OnEnable()
    {
        EventManager.StartListening("UserClick", UserClick);
    }

    private void OnDisable()
    {
        EventManager.StopListening("UserClick", UserClick);
    }

    public void BuildSelectedTower()
    {
        myTree = transform.GetComponentInParent<TowerTree>();
        myTree.Build(towerPrefab);
    }

    private void UserClick(GameObject obj, string param)
    {
        if (obj == gameObject)
            if (towerPrefab.name == "Destroy")
                Destroy(GameObject.FindWithTag("Tree"));
            else if (towerPrefab.name != "Destroy")
                BuildSelectedTower();
    }

    public void Destroy()
    {
        Destroy(GameObject.FindWithTag("Tree"));
    }
}
