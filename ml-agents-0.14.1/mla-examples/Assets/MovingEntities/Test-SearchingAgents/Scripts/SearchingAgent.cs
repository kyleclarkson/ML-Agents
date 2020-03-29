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
    Vector3 dstPrevious;

    public float maxSpeed;
    public float agentSpeed;

    public override void InitializeAgent() {
        m_Rb = GetComponent<Rigidbody>();
        targets = GameObject.FindGameObjectsWithTag("data_point");
        Debug.Log("Targets found: " + targets.Length);
        // Set random target point.
        SetNewTarget();
        dstPrevious = m_Rb.transform.position - currentTarget.transform.position;
    }

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

    // Note: Raycasts are automatically added to observations. 
    public override void CollectObservations() {

        // Position of target and agent
        AddVectorObs(currentTarget.transform.position);
        AddVectorObs(m_Rb.transform.position);

        // Velocity of agent
        AddVectorObs(m_Rb.velocity);
    }
    
    public override void AgentAction(float[] vectorAction) {

        if (m_Rb.transform.position.y < 0) {
            SetReward(-1f);
            m_Rb.position = new Vector3(0, 1f, 0);
            m_Rb.velocity = Vector3.zero;
            m_Rb.angularVelocity = Vector3.zero;
            SetNewTarget();
        }

        // Move agent.
        var directionToMove = Vector3.zero;
        var directionToRotate = Vector3.zero;

        var action = Mathf.FloorToInt(vectorAction[0]);

        switch (action) {
            case 1:
                directionToMove = transform.forward * 1f;
                break;
            case 2:
                directionToMove = transform.forward * -1f;
                break;
            case 3:
                directionToRotate = transform.up * 1f;
                break;
            case 4:
                directionToRotate = transform.up * -1f;
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
        Vector3 dstToTarget = (m_Rb.position - currentTarget.transform.position);
        float dstReward = dstToTarget.magnitude / dstPrevious.magnitude;
        
        AddReward(-dstReward);

        // Not done reward
        // AddReward(-0.0001f);
    }

    public override void AgentReset() {
        
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.CompareTag("target") || collision.gameObject.CompareTag("data_point")) {
            Debug.Log("Collision with : " + collision.gameObject.name);

            if (collision.gameObject.Equals(currentTarget)) {
                Debug.Log("Target found!");
                SetReward(10f);
                SetNewTarget();
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

        // Set dst to target from current position.
        dstPrevious = m_Rb.transform.position - currentTarget.transform.position;


    }
}
