using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlaceManager : MonoBehaviour
{
    public List<shipLap> players;

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        var p = players;
        players = players.OrderByDescending(x => x.currentLap).ThenByDescending(x => x.currentCheckpoint).ThenBy(x => x.distToNext).ToList();
        //players = tempPlayers.ToList();
        if (p.SequenceEqual(players)==false)
        {
            print("changed list");
        }
    }

    public void spawn()
    {
        print("add");

        /*
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Player").ToArray<GameObject>();
        List<shipLap> p = new List<shipLap>();
        for (var i = 0; i < temp.Length; i++)
        {
            p.Add(temp[i].GetComponent<shipLap>());
        }
        players = p;
        */

        //players = GameObject.FindObjectsOfType<shipLap>().ToList<shipLap>().OrderBy(x => x.currentLap).ThenBy(x => x.currentCheckpoint).ThenByDescending(x => x.distToNext).ToList();
        players = GameObject.FindObjectsOfType<shipLap>().ToList<shipLap>();
    }

    public GameObject findNext(GameObject s)
    {
        var i = players.IndexOf(s.GetComponent<shipLap>());
        if (i == 0)
        {
            return players[players.Count - 1].gameObject;
        }
        else
        {
            return players[i - 1].gameObject;
        }
    }
}
