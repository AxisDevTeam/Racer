using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using TMPro;

public class MultiplayerManager : NetworkBehaviour
{
    //public PlaceManager pm;
    // Start is called before the first frame update

    //[SyncVar]
    public string username;

    public TMP_Text usernameTXT;
    public TMP_InputField usernameField;
    void Start()
    {
        //pm = GameObject.Find("EventSystem []").GetComponent<PlaceManager>();
    }

    void Awake()
    {
        usernameTXT = transform.GetChild(0).GetChild(0).Find("Username Parent").GetChild(0).GetComponent<TMP_Text>();
        usernameField = GameObject.Find("Username Field").GetComponent<TMP_InputField>();
        usernameField.onSubmit.AddListener(_input_OnSubmit);
    }

    // Update is called once per frame
    void Update()
    {
        if (!base.IsOwner)
        {
            GetComponent<ShipController>().enabled = false;
            GetComponent<ShipBoost>().enabled = false;
            //GetComponent<shipLap>().enabled = false;
            GetComponent<ShipItem>().enabled = false;
            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

        }
        else
        {
            GetComponent<ShipController>().vcam.Follow = transform.GetChild(0);
            GetComponent<ShipController>().vcam.LookAt = transform.GetChild(0);
        }

        /*
        if(username.Equals(""))
        {
            username = base.OwnerId.ToString();
        }

        if (usernameField.text.Equals(""))
        {
            username = base.OwnerId.ToString();
        }
        else
        {
            if (username != usernameField.text)
            {
                username = usernameField.text;
            }
        }
        usernameTXT.text = username;
        */
    }

    [TargetRpc]
    public void RPCResetPos(NetworkConnection conn)
    {
        conn.FirstObject.GetComponent<FallManager>().revertToSafePosition();
    }

    [TargetRpc]
    public void RPCSwitchPos(NetworkConnection conn, Vector3 pos, Vector3 dir)
    {
        conn.FirstObject.transform.position = pos;
        conn.FirstObject.transform.up = dir;
    }

    [TargetRpc]
    public void RPCStealBoost(NetworkConnection conn)
    {
        conn.FirstObject.GetComponent<ShipBoost>().boostAmount = 0;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        SetName();
        PlayerNameTracker.OnNameChange += PlayerNameTracker_OnNameChange;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        PlayerNameTracker.OnNameChange -= PlayerNameTracker_OnNameChange;
    }

    private void PlayerNameTracker_OnNameChange(NetworkConnection arg1, string arg2)
    {
        if(arg1 != base.Owner)
        {
            return;
        }

        SetName();
    }

    private void SetName()
    {
        string result = null;
        if (base.Owner.IsValid)
            result = PlayerNameTracker.GetPlayerName(base.Owner);

        if (string.IsNullOrEmpty(result))
            result = base.OwnerId.ToString();

        usernameTXT.text = result;
    }
    private void _input_OnSubmit(string text)
    {
        PlayerNameTracker.SetName(text);
    }
}