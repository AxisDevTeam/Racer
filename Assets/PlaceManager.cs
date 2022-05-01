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
        players = players.OrderBy(x => x.currentLap).ThenBy(y => y.currentCheckpoint).ThenByDescending(y => y.distToNext).ToList();
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

        players = GameObject.FindObjectsOfType<shipLap>().ToList<shipLap>();
    }
}
