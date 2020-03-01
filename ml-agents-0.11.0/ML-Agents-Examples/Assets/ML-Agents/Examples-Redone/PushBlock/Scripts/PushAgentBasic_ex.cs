using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PushAgentBasic_ex : Agent {

    // The surface of the game area. 
    public GameObject ground;
    public GameObject area;

    [HideInInspector]
    public Bounds areaBounds;


    PushBlockAcademy_ex m_Academy;

    // Goal to which block needs to be moved to.
    public GameObject goal;
    // The block that needs to be moved.
    public GameObject block;

    // Detects when block reaches the goal area. 
    [HideInInspector]
    public GoalDetect_ex goalDetect;

    public bool usingVectorObs;

    // Used to change ground material given success or failure of agent.
    Renderer m_GroundRenderer;
    Material m_GroundMaterial;

    Rigidbody m_BlockRb;
    Rigidbody m_AgentRb;
    RayPerception m_RayPerception;

    float[] m_RayAngles = { 0f, 45f, 70f, 90f, 110f, 135f, 180f };
    string[] m_DectableObjects = { "block", "goal", "wall" };


    void Awake() {
        m_Academy = FindObjectOfType<PushBlockAcademy_ex>();    
    }

    public override void InitializeAgent() {
        base.InitializeAgent();

        // Setup goal detection for this agent.
        goalDetect = block.GetComponent<GoalDetect_ex>();
        goalDetect.agent = this;

        m_AgentRb = GetComponent<Rigidbody>();
        m_BlockRb = GetComponent<Rigidbody>();

        areaBounds = ground.GetComponent<Collider>().bounds;

        m_GroundRenderer = ground.GetComponent<Renderer>();
        m_GroundMaterial = m_GroundRenderer.material;

        SetResetParameters();
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

    public override void CollectObservations() {

        if (usingVectorObs) {
            var rayDistance = 12;

            // Last two arguments start/end offests. 
            AddVectorObs(m_RayPerception.Perceive(rayDistance, m_RayAngles, m_DectableObjects, 0f, 0f));
            AddVectorObs(m_RayPerception.Perceive(rayDistance, m_RayAngles, m_DectableObjects, 1.5f, 0f));
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction) {

        // Move agent
        MoveAgent(vectorAction);

        AddReward(-1f / agentParameters.maxStep);
    }

    public void SetResetParameters() {
        // TODO 
    }

    /// <summary>
    /// Rotate and move the agent.
    /// </summary>
    /// <param name="actionVector"></param>
    private void MoveAgent(float[] actionVector) {
        var directionToMove = Vector3.zero;
        var rotateDirection = Vector3.zero;
        var action = Mathf.FloorToInt(actionVector[0]);

        switch (action) {
            // Forward
            case 1:
                directionToMove = transform.forward * 1f;
                break;
            // Backward
            case 2:
                directionToMove = transform.forward * -1f;
                break;
            // 
            case 3:
                rotateDirection = transform.up * 1f;
                break;
            case 4:
                rotateDirection = transform.up * -1f;
                break;
            case 5:
                directionToMove = transform.right * -0.75f;
                break;
            case 6:
                directionToMove = transform.right * 0.75f;
                break;
        }

        // Rotate agent over time.
        transform.Rotate(rotateDirection, Time.fixedDeltaTime * 200f);
        // Move agent.
        m_AgentRb.AddForce(directionToMove * m_Academy.agentRunSpeed, ForceMode.VelocityChange);
    }
}
