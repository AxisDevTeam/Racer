using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class shipLap : NetworkBehaviour
{
    //[SyncVar]
    public int currentLap = 0;
    //[SyncVar]
    public int currentCheckpoint = 0;

    public float distToNext;
    public bool onLast;
    public PlaceManager pm;

    public List<TrackCheckpoint> checkpoints;

    public TMP_Text placeText;
    public TMP_Text lapText;
    public TMP_Text leaderboard;

    public ShipPlaceTracker spt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        pm = GameObject.Find("EventSystem []").GetComponent<PlaceManager>();
        pm.spawn();

        //checkpoints = GameObject.FindGameObjectsWithTag("checkpoint");
        checkpoints = GameObject.FindObjectsOfType<TrackCheckpoint>().ToList<TrackCheckpoint>();
        checkpoints = checkpoints.OrderBy(x => x.checkpointIndex).ToList();
        placeText = GameObject.Find("Place Text").GetComponent<TMP_Text>();
        lapText = GameObject.Find("Lap Text").GetComponent<TMP_Text>();
        leaderboard = GameObject.Find("Leaderboard").GetComponent<TMP_Text>();
        spt = GameObject.Find("ShipPlaceTracker").GetComponent<ShipPlaceTracker>();
    }

    void Update()
    {
        
        if (onLast == false)
        {
            distToNext = Vector3.Distance(transform.position, checkpoints[currentCheckpoint + 1].gameObject.GetComponent<Collider>().ClosestPoint(transform.position));
        }
        else
        {
            distToNext = Vector3.Distance(transform.position, checkpoints[0].gameObject.GetComponent<Collider>().ClosestPoint(transform.position));
        }

        if (GetComponent<MultiplayerManager>().NetworkObject.IsOwner)
        {
            placeText.text = (pm.players.IndexOf(this) + 1).ToString();
            lapText.text = currentLap.ToString();
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                currentLap = 0;
                currentCheckpoint = 0;
            }
            string s = "";
            foreach (var p in pm.players)
            {
                if (p.IsOwner)
                {
                    s += "YOU | ";
                }
                s += (HelperMethods.addOrdinal(pm.players.IndexOf(p) + 1)) + " place | current lap: " + p.currentLap.ToString() + "\n";
            }
            leaderboard.text = s;
        }

        if(Mathf.Round(transform.position.x) ==0 && Mathf.Round(transform.position.z)==0)
        {
            currentLap = 0;
            currentCheckpoint = 0;
            if (IsOwner)
            {
                spt.ChangeCheckpoint(this);
            }
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
        {
            //return;
        }
        if (other.GetComponent<TrackCheckpoint>())
        {


            if (Mathf.Abs(other.GetComponent<TrackCheckpoint>().checkpointIndex - currentCheckpoint) == 1)
            {
                currentCheckpoint = other.GetComponent<TrackCheckpoint>().checkpointIndex;
                onLast = other.GetComponent<TrackCheckpoint>().last == true;
            }

            if (onLast)
            {
                if (other.GetComponent<TrackCheckpoint>().checkpointIndex == 0)
                {
                    currentLap += 1;
                    currentCheckpoint = 0;
                    onLast = false;
                }
            }

            if (other.GetComponent<TrackCheckpoint>().last == false)
            {
                onLast = false;
            }

            FallManager fm = GetComponent<FallManager>();
            fm.lastSafePosition = transform.position;
            fm.lastSafeUpVector = transform.up;

            if (IsOwner) { 
            spt.ChangeCheckpoint(this);
            }
        }
    }

    
    public void structDataSet(ShipPlaceTracker.shipLapData sld) 
    {
        currentLap = sld.l ;
        currentCheckpoint = sld.cp;
        onLast = sld.oL;
        print("info set | id: " + this.OwnerId + ", uname: " + GetComponent<MultiplayerManager>().usernameTXT.text);
    }
    
}
