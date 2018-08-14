using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {

    public GameObject defenderButton;

    public GameObject attackerButton;

    [HideInInspector]
    public bool readyDef, readyAtk;

    private void Start()
    {
        readyDef = readyAtk = false;
    }

    public void NotReady()
    {
        readyDef = readyAtk = false;
        GameObject.FindWithTag("Attacker").GetComponent<PlayerHandler>().RpcCreateButton();
        AttackerButton();
    }

    public void DefenderButton()
    {
        GameObject myButton = Instantiate(defenderButton, defenderButton.transform.position, defenderButton.transform.rotation) as GameObject;

        myButton.transform.SetParent(gameObject.transform);

        readyDef = false;
    }

    public void AttackerButton()
    {
        GameObject myButton = Instantiate(attackerButton, gameObject.transform.position, gameObject.transform.rotation) as GameObject;

        myButton.transform.SetParent(gameObject.transform);

        readyAtk = false;
    }
}
