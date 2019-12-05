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
        return base.Heuristic();
    }

    public override void CollectObservations() {
        base.CollectObservations();
    }

    public override void AgentAction(float[] vectorAction, string textAction) {
        base.AgentAction(vectorAction, textAction);
    }

    public void SetResetParameters() {
        // TODO 
    }
}
