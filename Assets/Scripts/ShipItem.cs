using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipItem : NetworkBehaviour
{
    public ItemHandler ih;
    public Item item;
    ShipController sc;
    public Image display;

    public PlaceManager pm;
    public MultiplayerManager mm;
    // Start is called before the first frame update
    void Awake()
    {
        sc = GetComponent<ShipController>();
        ih = GameObject.Find("EventSystem []").GetComponent<ItemHandler>();
        display = GameObject.Find("Item Image").GetComponent<Image>();

        pm = GameObject.Find("EventSystem []").GetComponent <PlaceManager>();
        mm = GetComponent<MultiplayerManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (item != null)
            {
                if (item.itemName.Equals("Boost"))
                {
                    StartCoroutine(sc.Boost(item.length));
                }
                if (item.itemName.Equals("Enemy Reset"))
                {
                    mm.RPCResetPos(pm.findNext(gameObject).GetComponent<MultiplayerManager>().Owner);
                }
                if (item.itemName.Equals("Switch"))
                {
                    Vector3 tempPos = transform.position;
                    Vector3 tempVector = transform.up;
                    var temp = pm.findNext(gameObject);
                    transform.position = temp.transform.position;
                    transform.up = temp.transform.up;
                    mm.RPCSwitchPos(temp.GetComponent<MultiplayerManager>().Owner,tempPos, tempVector);

                }
                if (item.itemName.Equals("Steal Boost"))
                {
                    GetComponent<ShipBoost>().addBoost(pm.findNext(gameObject).GetComponent<ShipBoost>().syncBoostAmount);
                    RPCServerStealBoost(pm.findNext(gameObject).GetComponent<MultiplayerManager>().Owner, GetComponent<MultiplayerManager>().Owner);
                    print(pm.findNext(gameObject).GetComponent<MultiplayerManager>().Owner.ClientId);
                }
                item = null;
            }
            
        }

        if (item != null)
        {
            display.sprite = item.image;
        }
        else
        {
            display.sprite = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("item"))
        {
            item = ih.randomItem();
        }
    }

    [ServerRpc]
    public void RPCServerStealBoost(NetworkConnection conn, NetworkConnection sender)
    {
        RPCClientStealBoost(conn, sender.ClientId);
    }
    
    [TargetRpc]
    public void RPCClientStealBoost(NetworkConnection conn, int sender)
    {
        //RpcServerSendBoost(conn, conn.FirstObject.GetComponent<ShipBoost>().boostAmount, sender);
        print("val1: " + conn.FirstObject.GetComponent<ShipBoost>().syncBoostAmount);
        //RPCClientSendBoost(NetworkManager.ClientManager.Clients[sender], conn.FirstObject.GetComponent<ShipBoost>().syncBoostAmount);
        //print("val1: " + conn.FirstObject.GetComponent<ShipBoost>().boostAmount);
        conn.FirstObject.GetComponent<ShipBoost>().zeroBoost();
        print(conn.ClientId);
        print("x");
    }

    
    [ServerRpc]
    public void RPCServerSendBoost(int sender, float amount)
    {
        RPCClientSendBoost(NetworkManager.ClientManager.Clients[sender], amount);
    }
    

    [TargetRpc]
    public void RPCClientSendBoost(NetworkConnection conn, float amount)
    {
        conn.FirstObject.GetComponent<ShipBoost>().addBoost(amount);
    }
    
}
