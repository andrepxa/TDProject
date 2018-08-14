using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Button : NetworkBehaviour {

    private CanvasManager cm;

    private void Start()
    {
        cm = GameObject.Find("Canvas").GetComponent<CanvasManager>();
    }

    public void StartWaveClient()
    {
            gameObject.SetActive(false);
    }

    public void StartWaveServer()
    {
            cm.readyAtk = true;
            gameObject.SetActive(false);
            GameObject.Find("Panel").SetActive(false);
    }





}
