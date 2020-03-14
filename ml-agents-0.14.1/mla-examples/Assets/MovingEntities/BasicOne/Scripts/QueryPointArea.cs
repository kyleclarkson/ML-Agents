using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class QueryPointArea : MonoBehaviour {


    // List of Datapoints.
    public int numOfDatapoints;
    // The data points in the scene.
    GameObject[] datapoints;

    void Awake() {
        // Get all data point in scene.
        datapoints = GameObject.FindGameObjectsWithTag("datapoint");
        
       
    }

    public void queryPoint(GameObject queriedPoint) {

    }

}
