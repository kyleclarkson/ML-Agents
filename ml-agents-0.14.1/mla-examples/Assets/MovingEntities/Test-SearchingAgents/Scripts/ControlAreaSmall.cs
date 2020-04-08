using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlAreaSmall : MonoBehaviour {

    public AgentQuerier[] agents;
    public Datapoint[] datapoints;

    public float timeElapsed;
    public float secondsElapsed;

    // Save agents starting position and rotation for reset
    Vector3[] initPos;
    Quaternion[] initAngles;

    // Start is called before the first frame update
    void Start() {

        agents = FindObjectsOfType<AgentQuerier>();
        datapoints = FindObjectsOfType<Datapoint>();

        Debug.Log("Agents: " + agents.Length + ", datapoints: " + datapoints.Length);

        initPos = new Vector3[agents.Length];
        initAngles = new Quaternion[agents.Length];

        for (int i = 0; i < agents.Length; i++) {
            initPos[i] = agents[i].transform.position;
            initAngles[i] = agents[i].transform.rotation;
        }
    }

    public float GetTotalTimeOverdue() {
        float send = 0;
        foreach (Datapoint datapoint in datapoints) {
            if(datapoint.IsOverdue()) {
                send += datapoint.TimeOverdue();
            }
        }
        // This will be a negative amount
        return send;
    }

    public void QueryDatapoint(Datapoint datapoint) {
        datapoint.timeLastQueried = timeElapsed;
        datapoint.timeSinceLastQueried = 0;
    }

    public void ResetArea() {
        for(int i=0; i<agents.Length; i++) {
            agents[i].transform.SetPositionAndRotation(initPos[i], initAngles[i]);
        }

        foreach(Datapoint datapoint in datapoints) {
            datapoint.resetTimes();
        }

        timeElapsed = 0;
        secondsElapsed = 0;
    }
    
    // Handles timing
    void FixedUpdate() {
        // Update time
        timeElapsed += Time.deltaTime;

        // A second has passed
        if (timeElapsed >= 1) {
            secondsElapsed++;
            timeElapsed = 0;
            // Update time per each data point
            foreach (Datapoint dp in datapoints) {
                dp.timeSinceLastQueried++;
            }
        }
    }
}
