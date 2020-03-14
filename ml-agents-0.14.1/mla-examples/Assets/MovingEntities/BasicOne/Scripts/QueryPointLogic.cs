using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class tracks the time since the data point was last querried. 
/// </summary>
public class QueryPointLogic : MonoBehaviour {

    public QueryPointArea myArea;
    public int periodUpperBound;
    [HideInInspector]
    public int timeSinceLastQueried;
    public int decayRate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
