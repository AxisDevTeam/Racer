using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAnimation : MonoBehaviour
{
    public float bobSpeed = 1.5f;
    public float bobAmp = 0.075f;
    public float bobDelay = 0;

    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = startPos + new Vector3(0, Mathf.Sin((Time.time + bobDelay) * bobSpeed) * bobAmp, 0);
    }
}
