using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace Redone {
    public class PushAgentBasic : Agent {

        public GameObject ground;

        public GameObject area;

        /// <summary>
        /// The boundary of the area.
        /// </summary>
        [HideInInspector]
        public Bounds areaBounds;

        PushBlockSettings m_PushBlockSettings;

        public GameObject goal;

        public GameObject block;

        Rigidbody m_BlockRb;        // Cached on init
        Rigidbody m_AgentRb;        // Cached on init
        Material m_GroundMaterial;  // Cached on Awake
        Renderer m_GroundRenderer;


        /// <summary>
        /// Detects when the block touches the goal. 
        /// </summary>
        [HideInInspector]
        public GoalDetect goalDetect;

        public bool useVectorObs;

        // Get PushBlockSettings from Scene object.
        void Start() {
            m_PushBlockSettings = FindObjectOfType<Redone.PushBlockSettings>();
            if (!m_PushBlockSettings) {
                Debug.Log("Not found");
            }
        }

        public override void Initialize() {
            goalDetect = block.GetComponent<GoalDetect>();
            goalDetect.agent = this;

            m_AgentRb = GetComponent<Rigidbody>();

            m_BlockRb = block.GetComponent<Rigidbody>();
            // Get boundary of the ground.
            areaBounds = ground.GetComponent<Collider>().bounds;
            // Needed to change material of ground.
            m_GroundRenderer = ground.GetComponent<Renderer>();
            // Set starting material
            m_GroundMaterial = m_GroundRenderer.material;

            SetResetParameters();
        }

        public override float[] Heuristic() {
            if (Input.GetKey(KeyCode.D)) {
                return new float[] { 6 };
            }
            if (Input.GetKey(KeyCode.W)) {
                return new float[] { 1 };
            }
            if (Input.GetKey(KeyCode.A)) {
                return new float[] { 5 };
            }
            if (Input.GetKey(KeyCode.S)) {
                return new float[] { 2 };
            }
            if (Input.GetKey(KeyCode.Q)) {
                return new float[] { 4 };
            }
            if (Input.GetKey(KeyCode.E)) {
                return new float[] { 3 };
            }
            return new float[] { 0 };
        }

        public override void OnActionReceived(float[] vectorAction) {

            MoveAgent(vectorAction);

            // Existince penalty
            AddReward(-1f / maxStep);
        }

        public override void OnEpisodeBegin() {
            // Reset 
            var rotation = Random.Range(0, 4);
            var rotationAngle = rotation * 90f;
            area.transform.Rotate(new Vector3(0, rotationAngle, 0f));



        }

        public void MoveAgent(float[] act) {

            var dirToGo = Vector3.zero;
            var rotateDir = Vector3.zero;

            var action = Mathf.FloorToInt(act[0]);
            
            switch (action) {
                case 1:
                    dirToGo = transform.forward * 1f;
                    break;
                case 2:
                    dirToGo = transform.forward * -1f;
                    break;
                case 3:
                    rotateDir = transform.up * 1f;
                    break;
                case 4:
                    rotateDir = transform.up * -1f;
                    break;
                case 5:
                    dirToGo = transform.right * -0.75f;
                    break;
                case 6:
                    dirToGo = transform.right * 0.75f;
                    break;
            }

            transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
            m_AgentRb.AddForce(dirToGo * m_PushBlockSettings.agentRunSpeed,
                ForceMode.VelocityChange);
        }

        public void ScoredAGoal() {
            // TODO
        }

        public void ResetBlock() {
            // TODO
        }

        public void SetResetParameters() {
            SetGroundMaterialFricition();
            SetBlockProperties();
        }

        public void SetBlockProperties() {

            var resetParams = Academy.Instance.FloatProperties;

            var scale = resetParams.GetPropertyWithDefault("block_scale", 2);

            // Set scale and drag of block.
            m_BlockRb.transform.localScale = new Vector3(scale, 0.75f, scale);
            m_BlockRb.drag = resetParams.GetPropertyWithDefault("block_drag", 0.5f);
        }

        // TODO  see what resetParams are. 
        public void SetGroundMaterialFricition() {

            var resetParams = Academy.Instance.FloatProperties;
            var groundCollider = ground.GetComponent<Collider>();

            groundCollider.material.dynamicFriction =
                resetParams.GetPropertyWithDefault("dynamic_friction", 0);
            groundCollider.material.staticFriction =
                resetParams.GetPropertyWithDefault("static_friciton", 0);
        }
    }
}
