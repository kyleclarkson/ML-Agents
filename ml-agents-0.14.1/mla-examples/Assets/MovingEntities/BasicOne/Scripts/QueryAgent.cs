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
    public float moveSpeed = 2;


    public override void InitializeAgent() {

        Debug.Log("Init agent called");
        base.InitializeAgent();

        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<QueryPointArea>();
        m_QueryPointSettings = FindObjectOfType<QueryPointSettings>();
    }


    public override void AgentAction(float[] action) {

        Debug.Log("agent action called");
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

        // Apply movement and rotatio
        m_AgentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
        transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);

        // Slow down agent if moving too fase.
        if (m_AgentRb.velocity.sqrMagnitude > 20f) {
            m_AgentRb.velocity *= 0.95f;
        }

    }

    public override void AgentReset() {
        base.AgentReset();
    }

    public override void CollectObservations() {
        base.CollectObservations();
    }

    /// <summary>
    /// The 4 axis correspond to:
    /// 0 - Moving forward/backward
    /// 1 - Moving left/right
    /// 2 - Rotating
    /// 3 - Shooting laser
    /// Player only has access to 0 and 2. 
    /// </summary>
    /// <returns></returns>
    public override float[] Heuristic() {

        Debug.Log("heuristic called");
        var action = new float[3];

        // Get KB input.
        if (Input.GetKey(KeyCode.D)) {
            action[2] = 2f;
        }
        if (Input.GetKey(KeyCode.W)) {
            action[0] = 1f;
        }
        if (Input.GetKey(KeyCode.A)) {
            action[2] = 1f;
        }
        if (Input.GetKey(KeyCode.S)) {
            action[0] = 2f;
        }
        return action;

    }


    // =========================
    // === End Agent methods ===
    // =========================
    private void OnTrigger(Collision collision) {
    }

    private void OnTriggerEnter(Collider collision) {
        // Collision with data point
        if (collision.gameObject.CompareTag("data_point")) {
            Debug.Log("Collision with: " + collision.gameObject.name);

        }
    }
}
