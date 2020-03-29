using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class QueryAgent : Agent {

    Rigidbody m_AgentRb;
    public GameObject area;
    QueryPointArea m_MyArea;
    QueryPointSettings m_QueryPointSettings;

    public bool useVectorObs;

    // Agent movement properties.
    public float turnSpeed = 300;
    public float moveSpeed = 3;


    public override void InitializeAgent() { 
        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<QueryPointArea>();
        m_QueryPointSettings = FindObjectOfType<QueryPointSettings>();
    }


    public override void AgentReset() {
        if (m_AgentRb.transform.position.y < 0) {
            this.transform.position = new Vector3(0, 3f, 0);
            this.m_AgentRb.velocity = Vector3.zero;
        }
    }


    public override void AgentAction(float[] vectorAction) {

        Vector3 moveDir = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        var action = Mathf.FloorToInt(vectorAction[0]);

        // Apply movement and rotation.
        switch (action) {
            case 1:
                moveDir = transform.forward * 1f;
                break;
            case 2:
                moveDir = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        m_AgentRb.AddForce(moveDir * 2f, ForceMode.VelocityChange);

        float[] plys = m_MyArea.getPlys();
        
        // Agent has fallen off
        if (m_AgentRb.transform.position.y < 0) {
            SetReward(-1f);
            Done();
        }


        float movementPen = Random.Range(-0.001f, -0.0001f);
        // Add small negative reward to move.
        AddReward(movementPen);
        
    }

    /// <summary>
    /// (4*3) Distance between agent and query point.
    /// (1*3) Location of agent.
    /// (1*3) Velocity of agent
    /// (4*1) time since last queried of each query point.
    /// Total: 22
    /// </summary>
    public override void CollectObservations() {
        GameObject[] queryPoints = m_MyArea.queryPointsObjects;

        // Add position of each query point.
        foreach (GameObject qp in m_MyArea.queryPointsObjects) {
            // Add location
            //AddVectorObs(transform.InverseTransformDirection(qp.transform.position).x);
            //AddVectorObs(transform.InverseTransformDirection(qp.transform.position).z);
            //AddVectorObs((qp.transform.position).x);
            //AddVectorObs((qp.transform.position).z);
            AddVectorObs(qp.transform.position - transform.position);

        }

        // Add position of agent.
        //AddVectorObs(transform.InverseTransformDirection(m_AgentRb.transform.position).x);
        //AddVectorObs(transform.InverseTransformDirection(m_AgentRb.transform.position).z);
        // Position of agent
        AddVectorObs(m_AgentRb.position);

        // Velocity of agent
        AddVectorObs(m_AgentRb.velocity);

        // Add time last querried of each query point.
        float[] plys = m_MyArea.getPlys();
        foreach (float ply in plys) {
            AddVectorObs(ply);
        }
    }

    /// <summary>
    /// The 1 axis correspond to:
    /// 0 - Do nothing. 
    /// 1/2 - Moving forward/backward
    /// 3/4 - Rotating left/right
    /// </summary>
    /// <returns></returns>
    public override float[] Heuristic() {
        if (Input.GetKey(KeyCode.D)) {
            return new float[] { 3 };
        }
        if (Input.GetKey(KeyCode.W)) {
            return new float[] { 1 };
        }
        if (Input.GetKey(KeyCode.A)) {
            return new float[] { 4 };
        }
        if (Input.GetKey(KeyCode.S)) {
            return new float[] { 2 };
        }
        return new float[] { 0 };
    }


    // =========================
    // === End Agent methods ===
    // =========================
    private void OnTriggerEnter(Collider collision) {
        // Collision with data point
        if (collision.gameObject.CompareTag("query_point")) {
            Debug.Log("Collision with querypoint " + collision.gameObject.name + 
              ", id: " + collision.gameObject.GetInstanceID());

            // The point that was querried.
            QueryPoint qp = collision.gameObject.GetComponent<QueryPoint>();

            // Compute reward
            Debug.Log("Reward: " + (Time.time - qp.timeLastQueried));
            AddReward(Time.time - qp.timeLastQueried);
            // Query point - set LTQ to now.
            m_MyArea.queryPoint(collision.gameObject.GetComponent<QueryPoint>().GetInstanceID());
            
        }
    }
}
