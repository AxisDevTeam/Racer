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
    private readonly SyncDictionary<NetworkConnection, shipLapData> _lapInfo = new SyncDictionary<NetworkConnection, shipLapData>();

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
        shipLapData sld = new shipLapData(lap);
        _lapInfo[sender] = sld;
        //print(_lapInfo.ToString());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        foreach(var (key, value) in _lapInfo)
        {
            var obj = key.FirstObject;
            if (!obj.IsOwner)
            {
                //print("")
                var sl = obj.GetComponent<shipLap>();
                sl.structDataSet(value);
            }
            //print(i.Key.FirstObject.GetComponent<shipLap>().distToNext + " | " + i.Value.distToNext);
        }

        */
        string p = "";
        foreach(var i in _lapInfo)
        {
            p += i.Key.ClientId + " | " + i.Value.checkpoint + "\n";
        }
        print(p);
    }

    public struct shipLapData
    {
        public int lap;
        public int checkpoint;
        public shipLapData(shipLap sl)
        {
            lap = sl.currentLap;
            checkpoint = sl.currentCheckpoint;
        }
    }
}
