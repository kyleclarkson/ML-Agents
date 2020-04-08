using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class SearchingAgent7 : Agent
{
    Rigidbody m_Rb;
    GameObject currentTarget;

    GameObject[] targets;

    bool targetFound;

    // The position of the agent when the target was assigned to it. 
    Vector3 posTargetAssigned;

    // Used to determine if agent is moving in direction of target. 
    Vector3 prevPosition;
    
    public float agentSpeed;

    public override void InitializeAgent() {
        m_Rb = GetComponent<Rigidbody>();
        targets = GameObject.FindGameObjectsWithTag("data_point");
        Debug.Log("Targets found: " + targets.Length);

        targetFound = false;
        prevPosition = m_Rb.transform.position;
        // Set random target point.
        SetNewTarget();
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

        // Position of target and agent (6 values)
        AddVectorObs(currentTarget.transform.position);
        AddVectorObs(m_Rb.transform.position);

        // Velocity of agent (3 values)
        AddVectorObs(transform.InverseTransformDirection(m_Rb.velocity));

        // Distance between agent and target (1value)
        AddVectorObs(Vector3.Magnitude(m_Rb.transform.position - currentTarget.transform.position));

        // Target has been found.
        // AddVectorObs(targetFound);
    }
    
    public override void AgentAction(float[] vectorAction) {

        if (m_Rb.transform.position.y < 0 || m_Rb.transform.position.y > 20) {
            m_Rb.position = new Vector3(0, 1f, 0);
            m_Rb.velocity = Vector3.zero;
            m_Rb.angularVelocity = Vector3.zero;
            SetReward(-5f);
            Done();
        }
        
        if(targetFound) {
            AddReward(100f);
            Done();
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

        // Set previous position, then apply movement.
        prevPosition = m_Rb.transform.position;
        
        m_Rb.transform.Rotate(directionToRotate, Time.deltaTime * 200f);
        m_Rb.AddForce(directionToMove * agentSpeed, ForceMode.VelocityChange);

        // Cos angle between optimal vection and moved vector
        Vector3 vToTarget = (currentTarget.transform.position - m_Rb.transform.position);
        float cosAngle = Vector3.Dot(vToTarget.normalized, directionToMove.normalized);
        // AddReward(cosAngle / 1000);

        // Distance to target reward
        // AddReward(cosAngle / Mathf.Sqrt((currentTarget.transform.position - m_Rb.transform.position).magnitude));

        // Not done reward
        AddReward(-1f / maxStep);
    }

    public override void AgentReset() {
        Debug.Log("Agent reset called");
        if(targetFound) {
            m_Rb.velocity = Vector3.zero;
            m_Rb.angularVelocity = Vector3.zero;
            targetFound = false;
        }

        m_Rb.transform.position = new Vector3(0, 2f, 0);
        SetNewTarget();
    }

    private void FixedUpdate() {

        // Debugging
        if (true) {
            Debug.DrawRay(m_Rb.transform.position, m_Rb.transform.forward * 30f, Color.red);

            Debug.DrawLine(m_Rb.transform.position, currentTarget.transform.position, Color.green);
            Debug.DrawLine(prevPosition, currentTarget.transform.position, Color.blue);
        }

    }

    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.CompareTag("wall")) {
            //Debug.Log("hitting wall");
            // AddReward(-1f / maxStep);
        }
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.CompareTag("target") || collision.gameObject.CompareTag("data_point")) {
            //Debug.Log("Collision with : " + collision.gameObject.name);
        }
        if (collision.gameObject.Equals(currentTarget)) {
            Debug.Log("Target found!");
            targetFound = true;
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

        targetFound = false;
    }

    
}
