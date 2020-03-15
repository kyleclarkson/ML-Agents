using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class QueryAgent : Agent {

    Rigidbody m_AgentRb;
    public GameObject area;
    QueryPointArea m_MyArea;
    QueryPointSettings m_QueryPointSettings;

    // Agent movement properties.
    public float turnSpeed = 300;
    public float moveSpeed = 3;


    public override void InitializeAgent() {
        base.InitializeAgent();

        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<QueryPointArea>();
        m_QueryPointSettings = FindObjectOfType<QueryPointSettings>();
        
    }


    public override void AgentAction(float[] action) {
        // Move agent.
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var forwardAxis = (int)action[0];
        var rightAxis = (int)action[1];
        var rotateAxis = (int)action[2];

        switch (forwardAxis) {
            case 1:
                dirToGo = transform.forward;
                break;
            case 2:
                dirToGo = -transform.forward;
                break;
        }

        switch (rightAxis) {
            case 1:
                dirToGo = transform.right;
                break;

            case 2:
                dirToGo = -transform.right;
                break;
        }

        switch (rotateAxis) {
            case 1:
                rotateDir = transform.up;
                break;
            case 2:
                rotateDir = -transform.up;
                break;
        }

        // Apply movement and rotation.
        m_AgentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
        transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);

        // Slow down agent if moving too fase.
        if (m_AgentRb.velocity.sqrMagnitude > 20f) {
            m_AgentRb.velocity *= 0.95f;
        }

        // Add reward
        float totalPly = 0;
        foreach(float ply in m_MyArea.getTimesSinceLastQueried()) {
            totalPly += ply;
        }
        print("Total ply: " + totalPly);
        AddReward(totalPly);

        // Add small negative reward to move.
        AddReward(-0.000001f);
        

    }

    public override void AgentReset() {
        base.AgentReset();
    }

    /// <summary>
    /// (4*2) x and z location of each query point.
    /// (1*2) Agent's x and x location.
    /// (4*1) time since last queried of each query point.
    /// Total: 16
    /// </summary>
    public override void CollectObservations() {
        base.CollectObservations();
        //Debug.Log("Time last queried: " + string.Join(",", m_MyArea.getTimesSinceLastQueried()));

        // Get location of each datapoint. 
        GameObject[] queryPoints = m_MyArea.queryPointsObjects;

        foreach(GameObject qp in m_MyArea.queryPointsObjects) {
            // Add location
            AddVectorObs(qp.transform.position.x);
            AddVectorObs(qp.transform.position.z);
        }

        AddVectorObs(m_AgentRb.transform.position.x);
        AddVectorObs(m_AgentRb.transform.position.z);

        var timesLastQuerried = m_MyArea.getTimesSinceLastQueried();
        foreach (float timeLastQuerried in timesLastQuerried) {
            AddVectorObs(timeLastQuerried);
        }

    }

    /// <summary>
    /// The 3 axis correspond to:
    /// 0 - Moving forward/backward
    /// 1 - Moving left/right
    /// 2 - Rotating
    /// Player only has access to 0 and 2. 
    /// </summary>
    /// <returns></returns>
    public override float[] Heuristic() {
        var action = new float[3];

        // Get KB input.
        if (Input.GetKey(KeyCode.D)) {
            action[2] = 1f;
        }
        if (Input.GetKey(KeyCode.W)) {
            action[0] = 2f;
        }
        if (Input.GetKey(KeyCode.A)) {
            action[2] = 2f;
        }
        if (Input.GetKey(KeyCode.S)) {
            action[0] = 1f;
        }
        return action;

    }


    // =========================
    // === End Agent methods ===
    // =========================
    private void OnTriggerEnter(Collider collision) {
        // Collision with data point
        if (collision.gameObject.CompareTag("query_point")) {
            Debug.Log("Collision with querypoint " + collision.gameObject.name + 
              ", id: " + collision.gameObject.GetInstanceID());

            // Query point.
            collision.gameObject.GetComponent<QueryPoint>().pointQueried();
            m_MyArea.queryPoint(collision.gameObject.GetComponent<QueryPoint>().GetInstanceID());

            AddReward(100f);
        }
    }
}
