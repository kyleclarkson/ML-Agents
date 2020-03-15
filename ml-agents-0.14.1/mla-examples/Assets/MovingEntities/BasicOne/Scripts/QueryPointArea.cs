using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class QueryPointArea : MonoBehaviour {

    // Number of query points in scene.
    public int numOfQueryPoints;

    public GameObject[] queryPoints;


    public void ResetQueryArea() {
        Debug.Log("QueryPointArea Start() called");

        // Set up each data point
        queryPoints = GameObject.FindGameObjectsWithTag("querypoint");
        Debug.Log("Num of qps: " + queryPoints.Length);
    }
}
