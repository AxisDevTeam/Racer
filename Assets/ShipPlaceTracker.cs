using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using FishNet.Object.Synchronizing;
using FishNet.Managing.Logging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ShipPlaceTracker : NetworkBehaviour
{
    [SyncObject]
    private readonly SyncDictionary<NetworkConnection, shipLap> _lapInfo = new SyncDictionary<NetworkConnection, shipLap>();

    private static ShipPlaceTracker _instance;

    private void Awake()
    {
        _instance = this;
    }

    [Client]
    public static void ChangeCheckpoint(shipLap lap)
    {
        _instance.ServerSetLapInfo(lap);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerSetLapInfo(shipLap lap, NetworkConnection sender = null)
    {
        _lapInfo[sender] = lap;
        print(_lapInfo.ToString());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var i in _lapInfo)
        {
            print(i.Value.distToNext);
        }
    }
}
