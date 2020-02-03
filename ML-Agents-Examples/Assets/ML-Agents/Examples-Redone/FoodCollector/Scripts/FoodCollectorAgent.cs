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
        if (useVectorObs) {
            const float rayDistance = 50f;
            float[] rayAnlges = {20f, 45f, 70f, 90f, 110f, 135f, 160f};

            string[] dectableObjects = {"food", "agent", "wall", "badFood", "frozenAgent"};
            AddVectorObs(m_RayPer.Perceive(
                rayDistance: rayDistance,               // distance to object   -> 1 value
                rayAngles: rayAnlges,                   // ray angles           -> 7 mutlipiers
                detectableObjects: dectableObjects,     // objects to detect    -> 5 values
                startOffset: 0f,                
                endOffset: 0f));

            // Total values = 36 = (1+5) * 6 ray angles TODO check. 

            var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);

            // float -> 1 value
            AddVectorObs(localVelocity.x);
            // float -> 1 value
            AddVectorObs(localVelocity.z);
            // int -> 1 value
            AddVectorObs(System.Convert.ToInt32(m_Frozen));
            // int -> 1 value
            AddVectorObs(System.Convert.ToInt32(m_Shoot));

            // Total overal values = 40 values. 
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction) {
        MoveAgent(vectorAction);
    }

    public override void AgentOnDone() {
        // Override
    }

    public override void AgentReset() {
        // TODO
    }

    // === Util methods ===
    public void MoveAgent(float[] act) {
        // TODO 
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("food")) {
            Satiate();
            collision.gameObject.GetComponent<FoodLogic>().OnEaten();
            // Add reward.
            AddReward(1f);
            if( contribute) {
                m_MyAcademy.totalScore += 1;
            }
        }

        if(collision.gameObject.CompareTag("badFood")) {
            Poison();
            collision.gameObject.GetComponent<FoodLogic>().OnEaten();

            AddReward(-1f);
            if (contribute) {
                m_MyAcademy.totalScore -= 1;
            }
        }
    }


    public void SetResetParams() {
        SetLaserLengths();
        SetAgentScale();
    }
    
    /// Set laser length using default value or from specified academy params.
    public void SetLaserLengths() {
        m_LaserLength = m_MyAcademy.resetParameters.TryGetValue("laser_length", out m_LaserLength) ? m_LaserLength : 1.0f;
    }
    

    /// Set scale of agent using default value or from specified academy params. 
    public void SetAgentScale() {
        float agentScale;
        agentScale = m_MyAcademy.resetParameters.TryGetValue("agent_scale", out agentScale) ? agentScale : 1.0f;
        gameObject.transform.localScale = new Vector3(agentScale, agentScale, agentScale);
    }

    
    // === Private Methods ===

    // Set satiated (full) effect.
    private void Satiate() {
        m_Satiated = true;
        m_EffectTime = Time.time;
        gameObject.GetComponentInChildren<Renderer>().material = goodMaterial;
    }

    // Set unstatiate (hungry) effect.
    private void Unsatiate() {
        m_Satiated = false;
        gameObject.GetComponentInChildren<Renderer>().material = normalMaterial;
    }

    // Set posioned effect.
    private void Poison() {
        m_Poisoned = true;
        m_EffectTime = Time.time;
        gameObject.GetComponentInChildren<Renderer>().material = badMaterial;
    }

    // Set unposioned effect.
    private void Unpoison() {
        m_Poisoned = false;
        gameObject.GetComponentInChildren<Renderer>().material = normalMaterial;
    }

    // Set frozen effect.
    private void Freeze() {
        // Tag agent as frozen. 
        gameObject.tag = "frozenAgent";
        m_Frozen = true;
        m_FrozenTime = Time.time;
        gameObject.GetComponentInChildren<Renderer>().material = frozenMaterial;
    }

    // Set unfrozen effect.
    private void Unfreeze() {
        // Untag agent as frozen.
        gameObject.tag = "agent";
        m_Frozen = false;
        gameObject.GetComponentInChildren<Renderer>().material = normalMaterial;
    }

    
    private Color32 ToColor(int hexVal) {
        var r = (byte)((hexVal >> 16) & 0xFF);
        var g = (byte)((hexVal >> 8) & 0xFF);
        var b = (byte)(hexVal & 0xFF);
        return new Color32(r, g, b, 255);
    }
}
