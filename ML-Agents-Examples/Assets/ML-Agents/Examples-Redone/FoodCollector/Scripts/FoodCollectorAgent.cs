using UnityEngine;
using MLAgents;

public class FoodCollectorAgent : Agent {

    private FoodCollectorAcademy m_MyAcademy;
    public GameObject area;
    FoodCollectorArea m_MyArea;

    bool m_Frozen;
    bool m_Poisoned;
    bool m_Satiated;
    bool m_Shoot;

    float m_FrozenTime;
    float m_EffectTime;

    Rigidbody m_AgentRb;
    private float m_LaserLength;
    public float agentTurnSpeed = 300;
    public float agentMoveSpeed = 3;

    public Material normalMaterial;
    public Material badMaterial;
    public Material goodMaterial;
    public Material frozenMaterial;

    public GameObject myLaser;
    public bool contribute;
    private RayPerception3D m_RayPer;
    public bool useVectorObs;


    // === Override Methods ===
    public override void InitializeAgent() {
        base.InitializeAgent();
        m_AgentRb = GetComponent<Rigidbody>();
        Monitor.verticalOffset = 1f;
        m_MyArea = area.GetComponent<FoodCollectorArea>();
        m_RayPer = GetComponent<RayPerception3D>();
        m_MyAcademy = GetComponent<FoodCollectorAcademy>();

        SetResetParams();
    }

    public override void CollectObservations() {
        base.CollectObservations();
    }

    public override void AgentAction(float[] vectorAction, string textAction) {
        base.AgentAction(vectorAction, textAction);
    }

    public override void AgentOnDone() {
        base.AgentOnDone();
    }

    public override void AgentReset() {
        base.AgentReset();
    }

    // === ===
    public void SetResetParams() {
        SetLaserLengths();
        SetAgentScale();
    }

    /// <summary>
    /// Set laser length using default value or from specified academy params.
    /// </summary>
    public void SetLaserLengths() {
        m_LaserLength = m_MyAcademy.resetParameters.TryGetValue("laser_length", out m_LaserLength) ? m_LaserLength : 1.0f;
    }

    /// <summary>
    /// Set scale of agent using default value or from specified academy params. 
    /// </summary>
    public void SetAgentScale() {
        float agentScale;
        agentScale = m_MyAcademy.resetParameters.TryGetValue("agent_scale", out agentScale) ? agentScale : 1.0f;
        gameObject.transform.localScale = new Vector3(agentScale, agentScale, agentScale);
    }


}
