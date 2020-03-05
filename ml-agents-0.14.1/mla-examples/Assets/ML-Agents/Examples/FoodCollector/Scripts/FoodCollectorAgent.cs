using UnityEngine;
using MLAgents;

namespace Examples {

    public class FoodCollectorAgent : Agent {
        FoodCollectorSettings m_FoodCollecterSettings;
        public GameObject area;
        FoodCollectorArea m_MyArea;
        bool m_Frozen;
        bool m_Poisoned;
        bool m_Satiated;
        bool m_Shoot;
        float m_FrozenTime;
        float m_EffectTime;

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

        // == Override methods ==
        public override void InitializeAgent() {
            base.InitializeAgent();

            m_AgentRb = GetComponent<Rigidbody>();
            Monitor.verticalOffset = 1f;
            m_MyArea = area.GetComponent<FoodCollectorArea>();
            m_FoodCollecterSettings = FindObjectOfType<FoodCollectorSettings>();

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
            base.AgentReset();
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


        // == Helper Methods ==
        public void SetResetParameters() {
            // TODO 
        }

        public void MoveAgent(float[] action) {
            // TODO
        }
    }
}
