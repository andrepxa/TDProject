using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTree : MonoBehaviour {

    [HideInInspector]
    public PlaceTower tower;

    public void Build(GameObject towerPrefab)
    {
        tower.Build(towerPrefab);
    }
}
