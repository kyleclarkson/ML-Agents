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
    float timeLastQueried;

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Difference between </returns>
    public float pointQueried() {
        float timeSinceLastQueired = Time.time - timeLastQueried;

        return queryPeroid - timeSinceLastQueired;
    }

    
    void Start(){
        // Create datapoint
        timeLastQueried = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
