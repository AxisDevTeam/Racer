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

    [SyncVar]
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
}