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

            // Total values = 49 = (5 detectables object + 1 hit/not + 1 distance to object) * 7 ray angles. 
            
            var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);

            // float -> 1 value
            AddVectorObs(localVelocity.x);
            // float -> 1 value
            AddVectorObs(localVelocity.z);
            // int -> 1 value
            AddVectorObs(System.Convert.ToInt32(m_Frozen));
            // int -> 1 value
            AddVectorObs(System.Convert.ToInt32(m_Shoot));

            // Total overal values = 53 values. 
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction) {
        MoveAgent(vectorAction);
    }

    public override void AgentOnDone() {
        // Override
    }

    public override void AgentReset() {
        Unfreeze();
        Unpoison();
        Unsatiate();

        m_Shoot = false;
        m_AgentRb.velocity = Vector3.zero;
        // Set laser in scene relative to agent. 
        myLaser.transform.localScale = new Vector3(
            Random.Range(-m_MyArea.range, m_MyArea.range),
            2f,
            Random.Range(-m_MyArea.range, m_MyArea.range)) + area.transform.position;

        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        SetResetParams();
    }

    // === Util methods ===
    public void MoveAgent(float[] act) {

        m_Shoot = false;

        // Unfreeze after 4 seconds.
        if(Time.time > m_FrozenTime + 4f && m_Frozen) {
            Unfreeze();
        }

        // New effect can occur after 0.5 seconds.
        if (Time.time > m_EffectTime + 0.5) {
            if(m_Poisoned) {
                Unpoison();
            }
            if(m_Satiated) {
                Unsatiate();
            }
        }

        var directionToGo = Vector3.zero;
        var rotateDirection = Vector3.zero;

        if(!m_Frozen) {
            var shootCommand = false;
            var forwardAxis = (int)act[0];
            var rightAxis = (int)act[1];
            var rotateAxis = (int)act[2];
            var shootAxis = (int)act[3];

            // Handle way in which agent should move (forward/backward, rotation) and shoot action.
            switch(forwardAxis) {
                case 1:
                    directionToGo = transform.forward;
                    break;
                case 2:
                    directionToGo = -transform.forward;
                    break;
            }

            switch (rightAxis) {
                case 1:
                    rotateDirection = -transform.up;
                    break;
                case 2:
                    rotateDirection = transform.up;
                    break;
            }

            switch (shootAxis) {
                case 1:
                    shootCommand = true;
                    break;
            }

            if (shootCommand) {
                m_Shoot = true;
                directionToGo *= 0.5f;
                m_AgentRb.velocity *= 0.75f;
            }

            m_AgentRb.AddForce(directionToGo * agentMoveSpeed, ForceMode.VelocityChange);
            transform.Rotate(rotateDirection, Time.fixedDeltaTime * agentTurnSpeed);

                
        // Slow down agent
        if (m_AgentRb.velocity.sqrMagnitude > 25f) {
                m_AgentRb.velocity *= 0.95f;
            }
        }

        // Agent shoots laser
        if(m_Shoot) {
            var myTransform = transform;

            myLaser.transform.localScale = new Vector3(1f, 1f, m_LaserLength);
            var target = myTransform.TransformDirection(RayPerception3D.PolarToCartesian(25f, 90f));

            Debug.DrawRay(myTransform.position, target, Color.red, 0f, true);
            // The object hit. 
            RaycastHit hit;

            if (Physics.SphereCast(transform.position, 2f, target, out hit, 25f)) {
                if (hit.collider.gameObject.CompareTag("agent")) {
                    hit.collider.gameObject.GetComponent<FoodCollectorAgent>().Freeze();
                }
            }
        } else {
            myLaser.transform.localScale = new Vector3(0f, 0f, 0f);
        }
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
