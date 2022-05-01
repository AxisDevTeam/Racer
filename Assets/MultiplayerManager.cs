using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerManager : NetworkBehaviour
{
    //public PlaceManager pm;
    // Start is called before the first frame update
    void Start()
    {
        //pm = GameObject.Find("EventSystem []").GetComponent<PlaceManager>();
    }

    void Awake()
    {
        //pm.spawn();
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
    }
}
