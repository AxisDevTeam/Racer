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

    //private static ShipPlaceTracker _instance;

    private void Awake()
    {
        //_instance = this;
    }

    [Client]
    public void ChangeCheckpoint(shipLap lap)
    {
        shipLapData sld = new shipLapData(lap);
        ServerSetLapInfo(sld);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerSetLapInfo(shipLapData lap, NetworkConnection sender = null)
    {
        
        _lapInfo[sender] = lap;


        //print dictionary to console
        string p = "";
        foreach (KeyValuePair<NetworkConnection, shipLapData> kvp in _lapInfo)
        {
            //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            p += string.Format("Key = {0}, Value = {1}", kvp.Key.ClientId, kvp.Value.cp) + " ";

            if (!kvp.Key.FirstObject.IsOwner)
            {
                kvp.Key.FirstObject.GetComponent<shipLap>().structDataSet(kvp.Value);
            }
        }
        print(p);
    }

    public struct shipLapData
    {
        public int l;
        public int cp;
        public shipLapData(shipLap sl)
        {
            l = sl.currentLap;
            cp = sl.currentCheckpoint;
        }
    }
}
