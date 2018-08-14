using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spell : MonoBehaviour {

    public GameObject summon;
    private bool _cooldown = false;
    private float _cooldownTimer;

    private UiManager _uimanager;

    public UiManager UiManager
    {
        get
        {
            return _uimanager;
        }

        set
        {
            _uimanager = value;
        }
    }

    public bool Cooldown
    {
        get
        {
            return _cooldown;
        }

        set
        {
            _cooldown = value;
        }
    }

    public float CooldownTimer
    {
        get
        {
            return _cooldownTimer;
        }

        set
        {
            _cooldownTimer = value;
        }
    }

    private void Start()
    {
       UiManager = GameObject.Find("GameManager").GetComponent<UiManager>();
    }

    private void OnEnable()
    {
        EventManager.StartListening("UserClick", UserClick);
    }

    private void OnDisable()
    {
        EventManager.StopListening("UserClick", UserClick);
    }

    private void UserClick (GameObject obj, string param)
    {
        Debug.Log("Testou click em " + gameObject.name);
        if (obj == gameObject)
            Summon();
    }

    private void Update()
    {
        if (CooldownTimer > 0)
        {
            Transform cd = gameObject.transform.FindChild("CD");
            cd.gameObject.SetActive(true);
            CooldownTimer -= Time.deltaTime;
            cd.gameObject.GetComponent<Text>().text = CooldownTimer.ToString(); 
        }
    }

    void ResetCooldown()
    {
        Cooldown = false;

        Transform cd = gameObject.transform.FindChild("CD");
        cd.gameObject.SetActive(false);
    }

    public void Summon()
    {
        if (UiManager.Win)
            return;

        if (Cooldown == false)
        {
            GameObject attacker = GameObject.Find("Attacker");

            if (attacker == null && GameObject.Find("Defender") == null)
                return;

            attacker.GetComponent<PlayerHandler>().CmdSpawnMob(gameObject.name);

            CooldownTimer = 1.5f;
            Invoke("ResetCooldown", 1.5f);
            Cooldown = true;
        }
    }	
}
