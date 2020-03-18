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

        dirToGo.x = action[0];
        dirToGo.z = action[1];

        // Apply movement and rotation.
        m_AgentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);

        // Slow down agent if moving too fase.
        if (m_AgentRb.velocity.sqrMagnitude > 20f) {
            m_AgentRb.velocity *= 0.95f;
        }

        // Add reward
        float totalPly = 0;
        foreach(QueryPoint qp in m_MyArea.queryPoints.Values) {
            if (qp.ply() < 0) {
                AddReward(-1f);
            }
        }

        // Add small negative reward to move.
        // AddReward(-0.000001f);

        // End episode if ply becomes large.
        //if(m_MyArea.totalPly() < -120) {
        //    Done();
        //}

        Debug.Log("Total ply: " + m_MyArea.totalPly());
        

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
    /// The 2 axis correspond to:
    /// 0 - Moving forward/backward
    /// 1 - Moving left/right
    /// </summary>
    /// <returns></returns>
    public override float[] Heuristic() {
        var action = new float[3];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
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

            // The point that was querried.
            QueryPoint qp = collision.gameObject.GetComponent<QueryPoint>();


            // Compute reward
            AddReward(Time.time - qp.timeLastQueried);
            // Query point - set LTQ to now.
            m_MyArea.queryPoint(collision.gameObject.GetComponent<QueryPoint>().GetInstanceID());
            
        }
    }
}
