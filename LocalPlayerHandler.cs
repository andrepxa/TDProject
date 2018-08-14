using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LocalPlayerHandler : NetworkBehaviour {

    public NetworkManager nm;

    private void Awake()
    {
        Debug.Log("Entrei");

        GameObject p = GameObject.FindWithTag("Defender");
        if (p != null)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (!p.GetComponent<NetworkIdentity>().isServer)
                {
                    p.tag = "Defender";
                    Debug.Log(p.name + " changed tag to " + p.tag);
                    break;
                }
            }
        }
    }

}
