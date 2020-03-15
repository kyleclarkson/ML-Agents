﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class QueryPointArea : MonoBehaviour {

    // Number of query points in scene.
    public int numOfQueryPoints;

    //GameObject[] queryPoints;
    Dictionary<float, QueryPoint> queryPoints;

    public void ResetQueryArea() {

        queryPoints = new Dictionary<float, QueryPoint>();

        // Set up all datapoints
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("query_point")) {
            float list_key = o.GetComponent<QueryPoint>().GetInstanceID();
            QueryPoint list_value = o.GetComponent<QueryPoint>();
            // Store in list.
            queryPoints.Add(list_key, list_value);
        }
        numOfQueryPoints = queryPoints.Count;
        Debug.Log("Query points found: " + queryPoints.Count);
    }

    public void queryPoint(float qpId) {
        queryPoints[qpId].pointQueried();
    }

    /// <summary>
    /// Return time since last data point was queired. 
    /// </summary>
    /// <returns></returns>
    public float[] getTimesSinceLastQueried() {
        float[] send = new float[numOfQueryPoints];
        QueryPoint[] qps = (new List<QueryPoint>(queryPoints.Values)).ToArray();
        for (int i=0; i<numOfQueryPoints; i++) {
            send[i] = qps[i].ply();
        }
        return send;
    }
}
