using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class shipLap : MonoBehaviour
{
    public int currentLap = 0;
    public int currentCheckpoint = 0;
    public float distToNext;
    public bool onLast;
    public PlaceManager pm;

    public List<TrackCheckpoint> checkpoints;

    public TMP_Text placeText;
    public TMP_Text lapText;
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

        if (GetComponent<MultiplayerManager>().IsHost)
        {
            placeText.text = (pm.players.IndexOf(this) + 1).ToString();
            lapText.text = currentLap.ToString();
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                currentLap = 0;
                currentCheckpoint = 0;
            }
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TrackCheckpoint>())
        {
            if (Mathf.Abs(other.GetComponent<TrackCheckpoint>().checkpointIndex-currentCheckpoint)==1)
            {
                currentCheckpoint = other.GetComponent<TrackCheckpoint>().checkpointIndex;
                onLast = other.GetComponent<TrackCheckpoint>().last == true;
            }

            if (onLast)
            {
                if(other.GetComponent<TrackCheckpoint>().checkpointIndex == 0)
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

        }
    }
}