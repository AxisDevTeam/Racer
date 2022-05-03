using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallManager : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector3 lastSafePosition = Vector3.zero + Vector3.one;
    public Vector3 lastSafeUpVector = Vector3.up;

    public float yBoundary;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < yBoundary)
        {
            revertToSafePosition();
        }
    }

    public void revertToSafePosition()
    {
        transform.position = lastSafePosition;
        transform.up = lastSafeUpVector;
        var sc = GetComponent<ShipController>();
        sc.currentSpeed = 0;
    }


}
