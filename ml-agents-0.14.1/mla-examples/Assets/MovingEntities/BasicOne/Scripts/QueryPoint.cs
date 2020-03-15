using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class tracks the time since the data point was last querried. 
/// </summary>
public class QueryPoint : MonoBehaviour {

    public QueryPointArea myArea;

    // Must be quired once within this frequencey.
    public int queryPeroid;
    [HideInInspector]
    public float timeLastQueried;

    void Start() {
        // Create datapoint
        timeLastQueried = Time.time;
    }

    public void pointQueried() {
        timeLastQueried = Time.time;
    }

    public float ply() {
        float timeSinceLastQueried = Time.time - timeLastQueried;
        return queryPeroid - timeSinceLastQueried;
    }
}
