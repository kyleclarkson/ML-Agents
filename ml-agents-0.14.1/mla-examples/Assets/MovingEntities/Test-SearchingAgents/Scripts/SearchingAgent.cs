using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class SearchingAgent : Agent
{
    Rigidbody m_Rb;
    GameObject currentTarget;

    GameObject[] targets;

    // The position of the agent when the target was assigned to it. 
    Vector3 posTargetAssigned;

    public float maxSpeed;
    public float agentSpeed;

    public override void InitializeAgent() {
        m_Rb = GetComponent<Rigidbody>();
        targets = GameObject.FindGameObjectsWithTag("data_point");
        Debug.Log("Targets found: " + targets.Length);
        // Set random target point.
        SetNewTarget();
    }

    public override float[] Heuristic() {
        //if (Input.GetKey(KeyCode.D)) {
        //    return new float[] { 3 };
        //}
        //if (Input.GetKey(KeyCode.W)) {
        //    return new float[] { 1 };
        //}
        //if (Input.GetKey(KeyCode.A)) {
        //    return new float[] { 4 };
        //}
        //if (Input.GetKey(KeyCode.S)) {
        //    return new float[] { 2 };
        //}
        //return new float[] { 0 };
        var action = new float[3];
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

    // Note: Raycasts are automatically added to observations. 
    public override void CollectObservations() {

        // Position of target and agent
        AddVectorObs(currentTarget.transform.position);
        AddVectorObs(m_Rb.transform.position);

        // Velocity of agent
        AddVectorObs(transform.InverseTransformDirection(m_Rb.velocity));
    }
    
    public override void AgentAction(float[] vectorAction) {

        //if (m_Rb.transform.position.y < 0) {
        //    SetReward(-1f);
        //    m_Rb.position = new Vector3(0, 1f, 0);
        //    m_Rb.velocity = Vector3.zero;
        //    m_Rb.angularVelocity = Vector3.zero;
        //    SetNewTarget();
        //}

        // Move agent.
        var directionToMove = Vector3.zero;
        var directionToRotate = Vector3.zero;

        var forwardAxis = (int)vectorAction[0];
        var rightAxis = (int)vectorAction[1];
        var rotateAxis = (int)vectorAction[2];

        switch (forwardAxis) {
            case 1:
                directionToMove = transform.forward;
                break;
            case 2:
                directionToMove = -transform.forward;
                break;
        }

        switch (rightAxis) {
            case 1:
                directionToMove = transform.right;
                break;
            case 2:
                directionToMove = -transform.right;
                break;
        }

        switch (rotateAxis) {
            case 1:
                directionToRotate = -transform.up;
                break;
            case 2:
                directionToRotate = transform.up;
                break;
        }

        transform.Rotate(directionToRotate, Time.deltaTime * 200f);
        m_Rb.AddForce(directionToMove * agentSpeed, ForceMode.VelocityChange);
        //if (m_Rb.velocity.magnitude <= maxSpeed) {
        //    m_Rb.AddForce(directionToMove * agentSpeed, ForceMode.VelocityChange);
        //} else {
        //    m_Rb.velocity = m_Rb.velocity.normalized * maxSpeed;
        //}

        // Distance to target reward
        Vector3 currentDstToTarget = (m_Rb.position - currentTarget.transform.position);
        Vector3 assignedDstToTarget = (posTargetAssigned - currentTarget.transform.position);
        float dstReward = currentDstToTarget.magnitude / assignedDstToTarget.magnitude;

        AddReward(-dstReward / maxStep);

        // Not done reward
        //AddReward(-1f / maxStep);
    }

    public override void AgentReset() {
        Debug.Log("Agent reset called");
        m_Rb.velocity = Vector3.zero;
        m_Rb.angularVelocity = Vector3.zero;

        m_Rb.transform.position = new Vector3(0, 2f, 0);
        SetNewTarget();
    }

    private void Update() {
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.CompareTag("target") || collision.gameObject.CompareTag("data_point")) {
            //Debug.Log("Collision with : " + collision.gameObject.name);

            if (collision.gameObject.Equals(currentTarget)) {
                Debug.Log("Target found!");
                AddReward(100f);
                Done();
            }
        }
    }

    // === Helper methods ===
    private void SetNewTarget() {
        if (currentTarget == null) {
            currentTarget = targets[Random.Range(0, targets.Length)];
            var targetRenderer = currentTarget.GetComponent<Renderer>();
            targetRenderer = currentTarget.GetComponent<Renderer>();
            targetRenderer.material.SetColor("_Color", Color.red);
            currentTarget.tag = "target";

        } else {
            var targetRenderer = currentTarget.GetComponent<Renderer>();
            GameObject newTarget;
            do {
                newTarget = targets[Random.Range(0, targets.Length)];
            } while (currentTarget == newTarget);
            // Reset colours and tags
            targetRenderer.material.SetColor("_Color", Color.green);
            currentTarget.tag = "data_point";
            currentTarget = newTarget;
            targetRenderer = currentTarget.GetComponent<Renderer>();
            targetRenderer.material.SetColor("_Color", Color.red);
            currentTarget.tag = "target";
        }

        // position of agent when assigned target.
        posTargetAssigned = m_Rb.transform.position;


    }
}
