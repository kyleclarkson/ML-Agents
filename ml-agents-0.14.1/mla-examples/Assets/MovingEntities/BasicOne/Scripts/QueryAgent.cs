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


    public override void AgentAction(float[] action) {

        var dirToGo = Vector3.zero;
        dirToGo.x = -action[0];
        dirToGo.z = -action[1];

        // Apply movement and rotation.
        m_AgentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);

        // Slow down agent if moving too fase.
        if (m_AgentRb.velocity.sqrMagnitude > 20f) {
            m_AgentRb.velocity *= 0.95f;
        }

        float[] plys = m_MyArea.getPlys();

        //foreach (float ply in plys) {
        //    if (ply < 0) {
        //        AddReward(-1f);
        //    }
        //}


        float movementPen = Random.Range(-0.001f, -0.0001f);
        // Add small negative reward to move.
        AddReward(movementPen);

        // End episode if ply becomes large.
        //if(m_MyArea.totalPly() < -120) {
        //    Done();
        //}
    }

    public override void AgentReset() {
        base.AgentReset();
    }

    /// <summary>
    /// (4*3) Distance between agent and query point.
    /// (1*3) Location of target.
    /// (4*1) time since last queried of each query point.
    /// Total: 19
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

        AddVectorObs(transform.position);

        // Add time last querried of each query point.
        float[] plys = m_MyArea.getPlys();
        foreach (float ply in plys) {
            AddVectorObs(ply);
        }
    }

    /// <summary>
    /// The 2 axis correspond to:
    /// 0 - Moving forward/backward
    /// 1 - Moving left/right
    /// </summary>
    /// <returns></returns>
    public override float[] Heuristic() {
        var action = new float[2];
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
            Debug.Log("Reward: " + (Time.time - qp.timeLastQueried));
            AddReward(Time.time - qp.timeLastQueried);
            // Query point - set LTQ to now.
            m_MyArea.queryPoint(collision.gameObject.GetComponent<QueryPoint>().GetInstanceID());
            
        }
    }
}
