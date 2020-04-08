using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class AgentQuerier : Agent {

    Rigidbody m_Rb;
    public ControlAreaSmall myArea;
    public float speed;
    public Datapoint currentTarget;

    public override void InitializeAgent() {
        myArea = FindObjectOfType<ControlAreaSmall>();
        currentTarget = null;
        m_Rb = GetComponent<Rigidbody>();
    }

    public override void AgentReset() {
    }

    public override void CollectObservations() {

        // for each agent (including this one)
        foreach(AgentQuerier agent in myArea.agents) {
            // 3 : Agent position
            AddVectorObs(agent.transform.position);
            // 3 : Agent's current target position
            if (agent.currentTarget != null) {
                AddVectorObs(agent.currentTarget.transform.position);
            } else {
                AddVectorObs(new Vector3(0f,-10f,0f));
            }
            // 1 : Agent's speed
            AddVectorObs(agent.speed);
        }
        // 4 * (3+3+1) = 28

        // for each data point 
        foreach(Datapoint datapoint in myArea.datapoints) {
            // 1 : Time since data point was last queried
            AddVectorObs(datapoint.TimeOverdue());
        }
        // 8 * 1 = 8
    }

    public override void AgentAction(float[] targetChoice) {

        // Determine if reset is needed. 
        if (myArea.GetTotalTimeOverdue() < -180) {
            Debug.Log("Reset - Total over due too low");
            SetReward(-10f);
            myArea.ResetArea();
            Done();
        }

        // Agent does not have target and is requesting one
        if (currentTarget == null && targetChoice[0] >-1 && targetChoice[0] < myArea.datapoints.Length) {
            currentTarget = myArea.datapoints[(int) targetChoice[0]];
        }

        // Move agent towards target
        if(currentTarget != null) {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, step);

            Debug.DrawLine(transform.position, currentTarget.transform.position, Color.green);
        }
        // Apply penalty for each data point over query
        foreach (Datapoint datapoint in myArea.datapoints) {
            if(datapoint.IsOverdue()) {
                AddReward(-2f / maxStep);
            }
        }
    }

    public override float[] Heuristic() {

        // value length+1 corresponds to no target
        float[] send = new float[1];
        send[0] = -1;
        int k = 0;

        if (Input.GetKey(KeyCode.Alpha1)) {
            send[k] = 0f;
        } else if (Input.GetKey(KeyCode.Alpha2)) {
            send[k] = 1f;
        } else if (Input.GetKey(KeyCode.Alpha3)) {
            send[k] = 2f;
        } else if (Input.GetKey(KeyCode.Alpha4)) {
            send[k] = 3f;
        } else if (Input.GetKey(KeyCode.Alpha5)) {
            send[k] = 4f;
        } else if (Input.GetKey(KeyCode.Alpha6)) {
            send[k] = 5f;
        } else if (Input.GetKey(KeyCode.Alpha7)) {
            send[k] = 6f;
        } else if (Input.GetKey(KeyCode.Alpha8)) {
            send[k] = 7f;
        } else if (Input.GetKey(KeyCode.G)){
            send[k] = 8f;
        }
       
        return send;
    }

    private void OnTriggerStay(Collider other) {
        if(currentTarget != null) {
            if (other.gameObject.name == currentTarget.name) {
                if(currentTarget.IsOverdue()) {
                    AddReward(0.5f / maxStep);
                }
                myArea.QueryDatapoint(currentTarget);
                currentTarget = null;
            }
        }
    }

    private string printArray(float[] array) {
        string send = "";

        for (int i=0;i<array.Length; i++) {
            send += array[i] + ", ";
        }
        return send;
    }
}
