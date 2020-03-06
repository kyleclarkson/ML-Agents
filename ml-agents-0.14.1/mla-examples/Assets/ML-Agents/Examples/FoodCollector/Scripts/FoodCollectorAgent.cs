using UnityEngine;
using MLAgents;

namespace Examples {

    public class FoodCollectorAgent : Agent {
        FoodCollectorSettings m_FoodCollecterSettings;
        public GameObject area;
        FoodCollectorArea m_MyArea;
        bool m_Frozen;      // if agent is frozen
        bool m_Poisoned;    // if agent is poisoned
        bool m_Satiated;    // if agent is fed
        bool m_Shoot;       // if agent can shoot
        float m_FrozenTime; // time agent was frozen at
        float m_EffectTime; // time effect occured (e.g. being feed, poisioned)

        Rigidbody m_AgentRb;
        float m_LaserLength;
        // Speed of agent rotation.
        public float turnSpeed = 300;
        // Speed of agent movement.
        public float moveSpeed = 2;

        public Material normalMaterial;
        public Material badMaterial;
        public Material goodMaterial;
        public Material frozenMaterial;
        public GameObject myLaser;

        public bool contribute;
        public bool useVectorObs;

        // ============================
        // == Agent Override methods ==
        // ============================

        public override void InitializeAgent() {
            base.InitializeAgent();

            m_AgentRb = GetComponent<Rigidbody>();
            Monitor.verticalOffset = 1f;
            m_MyArea = area.GetComponent<Examples.FoodCollectorArea>();
            m_FoodCollecterSettings = FindObjectOfType<Examples.FoodCollectorSettings>();
            
            SetResetParameters();
        }
        public override void CollectObservations() {
            if (useVectorObs) {
                var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
                AddVectorObs(localVelocity.x);
                AddVectorObs(localVelocity.z);
                AddVectorObs(System.Convert.ToInt32(m_Frozen));
                AddVectorObs(System.Convert.ToInt32(m_Shoot));
            }
        }

        public override void AgentReset() {
            Unfreeze();
            Unpoison();
            Unsatiate();

            m_Shoot = false;
            m_AgentRb.velocity = Vector3.zero;

            myLaser.transform.localScale = new Vector3(0f, 0f, 0f);

            transform.position = new Vector3(
                Random.Range(-m_MyArea.range, m_MyArea.range),
                2f,
                Random.Range(-m_MyArea.range, m_MyArea.range))
                + area.transform.position;
            transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

            SetResetParameters();
        }

        public override void AgentAction(float[] vectorAction) {
            MoveAgent(vectorAction);
        }

        /// <summary>
        /// The 4 axis correspond to:
        /// 0 - Moving forward/backward
        /// 1 - Moving left/right
        /// 2 - Rotating
        /// 3 - Shooting laser
        /// Player only has access to 0,2 and 3. 
        /// </summary>
        /// <returns></returns>
        public override float[] Heuristic() {
            var action = new float[4];

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
            action[3] = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
            return action;
        }

        /// <summary>
        /// Given action vector, apply corresponding actions.
        /// The 4 axis correspond to:
        /// 0 - Moving forward/backward
        /// 1 - Moving left/right
        /// 2 - Rotating
        /// 3 - Shooting laser
        /// </summary>
        /// <param name="action">A len 4 array</param>
        public void MoveAgent(float[] action) {

            m_Shoot = false;

            // Enough time has passed since being frozen. 
            if (Time.time > m_FrozenTime + 4f && m_Frozen) {
                Unfreeze();
            }
            if (Time.time > m_EffectTime + 0.5f) {
                if (m_Poisoned) {
                    Unpoison();
                }
                if (m_Satiated) {
                    Unsatiate();
                }
            }

            var dirToGo = Vector3.zero;
            var rotateDir = Vector3.zero;

            if (!m_Frozen) {
                // Agent can move. Determine how from action vector.

                var forwardAxis = (int)action[0];
                var rightAxis = (int)action[1];
                var rotateAxis = (int)action[2];
                var shootAxis = (int)action[3];
                var shootCommand = false;

                switch (forwardAxis) {
                    // Forward
                    case 1:
                        dirToGo = transform.forward;
                        break;
                    // Backward
                    case 2:
                        dirToGo = -transform.forward;
                        break;
                }

                switch (rightAxis) {
                    // Go left
                    case 1:
                        dirToGo = transform.right;
                        break;
                    // Go right
                    case 2:
                        dirToGo = -transform.right;
                        break;
                }

                switch (rotateAxis) {
                    // Rotate down
                    case 1:
                        rotateDir = -transform.up;
                        break;
                    // Rotate up
                    case 2:
                        rotateDir = transform.up;
                        break;
                }

                switch (shootAxis) {
                    case 1:
                        shootCommand = true;
                        break;
                }

                if (shootCommand) {
                    // Agent shoots, scale down direction and speed
                    m_Shoot = true;
                    dirToGo *= 0.5f;
                    m_AgentRb.velocity *= 0.75f;
                }

                // Apply vectors (i.e. force) to agent.
                m_AgentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
                transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);

            }

            // Slow down agent.
            if (m_AgentRb.velocity.sqrMagnitude > 25f) {
                m_AgentRb.velocity *= 0.95f;
            }
            
            if (m_Shoot) {
                // Get ray
                var myTransform = transform;
                myLaser.transform.localScale = new Vector3(1f, 1f, m_LaserLength);
                var rayDir = 25.0f * myTransform.forward;
                // Draw ray and determine if it hits an agent.
                // (Should not occur with single agent)
                Debug.DrawRay(myTransform.position, rayDir, Color.red, 0f, transform);
                RaycastHit hit;

                if (Physics.SphereCast(transform.position, 2f, rayDir, out hit, 25f)) {
                    if (hit.collider.gameObject.CompareTag("agent")) {
                        // Freeze hit agent. 
                        hit.collider.gameObject.GetComponent<FoodCollectorAgent>().Freeze();
                    }
                }

            } else {
                myLaser.transform.localScale = new Vector3(0f, 0f, 0f);
            }
        }

        // A collision event is detected.
        private void OnCollisionEnter(Collision collision) {

            // Collision with food item
            if (collision.gameObject.CompareTag("food")) {
                Satiate();
                collision.gameObject.GetComponent<Examples.FoodLogic>().OnEaten();

                AddReward(1f);
                if (contribute) {
                    m_FoodCollecterSettings.totalScore += 1;
                }
            }

            // Collision with badFood item
            if (collision.gameObject.CompareTag("badFood")) {
                Poison();
                collision.gameObject.GetComponent<Examples.FoodLogic>().OnEaten();

                AddReward(-1.0f);
                if (contribute) {
                    m_FoodCollecterSettings.totalScore -= 1;
                }
            }
        }

        // ====================
        // == Helper Methods ==
        // ====================

        void Freeze() {
            gameObject.tag = "frozenAgent";
            m_Frozen = true;
            m_FrozenTime = Time.time;
            gameObject.GetComponentInChildren<Renderer>().material = frozenMaterial;
        }

        void Unfreeze() {
            m_Frozen = false;
            gameObject.tag = "agent";
            gameObject.GetComponentInChildren<Renderer>().material = normalMaterial;
        }

        void Poison() {
            m_Poisoned = true;
            m_EffectTime = Time.time;
            gameObject.GetComponentInChildren<Renderer>().material = badMaterial;
        }

        void Unpoison() {
            m_Poisoned = false;
            gameObject.GetComponentInChildren<Renderer>().material = normalMaterial;
        }

        void Satiate() {
            m_Satiated = true;
            m_EffectTime = Time.time;
            gameObject.GetComponentInChildren<Renderer>().material = goodMaterial;
        }

        void Unsatiate() {
            m_Satiated = false;
            gameObject.GetComponentInChildren<Renderer>().material = normalMaterial;
        }

        public void SetResetParameters() {
            SetLaserLengths();
            SetAgentScale();
        }

        public void SetLaserLengths() {
            m_LaserLength = Academy.Instance.FloatProperties.GetPropertyWithDefault("laser_length", 1.0f);
        }

        public void SetAgentScale() {
            float agentScale = Academy.Instance.FloatProperties.GetPropertyWithDefault("agent_scale", 1.0f);
            gameObject.transform.localScale = new Vector3(agentScale, agentScale, agentScale);
        }
    }
}
