using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAnimation : MonoBehaviour
{
    public float bobSpeed = 1.5f;
    public float bobAmp = 0.075f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(0, Mathf.Sin(Time.time * bobSpeed) * bobAmp, 0);
    }
}
