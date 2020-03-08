using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using Barracuda; // For NNModel classes

namespace Examples {

    public class WallJumpAgent : Agent {

        // Value determines height of wall.
        int m_Configuration;

        // Brains depending on wall.
        public NNModel noWallBrain;
        public NNModel smallWallBrain;
        public NNModel bigWallBrain;

        public GameObject ground;
        public GameObject spawnArea;
        Bounds m_SpawnAreaBounds;

        public GameObject goal;
        public GameObject shortBlock;
        public GameObject wall;

        Rigidbody m_AgentRb;
        Rigidbody m_ShortBlockRb;

        Material m_GroundMaterial;
        Renderer m_GroundRenderer;

        WallJumpSettings m_WallJumpSettings;

        public float jumpingTime;
        public float jumpTime;

        Vector3 m_JumpTargetPos;
        Vector3 m_JumpStartingPos;

        // A downward force to make jumps less floaty
        public float fallingForce;

        // Use to check the coliding objects
        public Collider[] hitGroundColliders = new Collider[3];

        // ==============================
        // === Agent override methods ===
        // ==============================
        public override void InitializeAgent() {
            m_WallJumpSettings = FindObjectOfType<WallJumpSettings>();
            m_Configuration = Random.Range(0, 5);

            m_AgentRb = GetComponent<Rigidbody>();
            m_ShortBlockRb = shortBlock.GetComponent<Rigidbody>();
            m_SpawnAreaBounds = spawnArea.GetComponent<Collider>().bounds;
            m_GroundRenderer = ground.GetComponent<Renderer>();
            m_GroundMaterial = m_GroundRenderer.material;

            spawnArea.SetActive(false);
        }

        public override float[] Heuristic() {
            var action = new float[4];
            if (Input.GetKey(KeyCode.D)) {
                action[1] = 2f;
            }
            if (Input.GetKey(KeyCode.W)) {
                action[0] = 1f;
            }
            if (Input.GetKey(KeyCode.A)) {
                action[1] = 1f;
            }
            if (Input.GetKey(KeyCode.S)) {
                action[0] = 2f;
            }
            action[3] = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
            return action;
        }

        public override void CollectObservations() {
            // Agents location relative to ground.
            var agentPosition = m_AgentRb.position - ground.transform.position;

            AddVectorObs(agentPosition / 20f);
            AddVectorObs(DoGroundCheck(true) ? 1 : 0);
        }

        public override void AgentAction(float[] vectorAction) {
            // Move agent
            MoveAgent(vectorAction);

            // Jump failed? TODo
            if ((!Physics.Raycast(m_AgentRb.position, Vector3.down, 20)) ||
                (!Physics.Raycast(m_ShortBlockRb.position, Vector3.down, 20))) {
                SetReward(-1f);
                Done();
                ResetBlock(m_ShortBlockRb);
                StartCoroutine(
                    GoalScoredSwapGroundMaterial(m_WallJumpSettings.goalFailedMaterial, .5f));
            }
        }

        // Reset agent and block's position.
        public override void AgentReset() {
            ResetBlock(m_ShortBlockRb);
            transform.localPosition = new Vector3(
                18 * (Random.value - 0.5f), 1, -12);
            m_Configuration = Random.Range(0, 5);
            m_AgentRb.velocity = default(Vector3);
        }
        // ==================================
        // === End Agent override methods ===
        // ==================================


        public void MoveAgent(float[] action) {
            // Apply neg award for not reaching goal.
            AddReward(-0.0005f);

            var smallGrounded = DoGroundCheck(true);
            var largeGrounded = DoGroundCheck(false);

            var dirToGo = Vector3.zero;
            var rotateDir = Vector3.zero;

            // Get actions passed by array.
            var dirToGoForwardAction = (int)action[0];
            var rotateDirAction = (int)action[1];
            var dirToGoSideAction = (int)action[2];
            var jumpAction = (int)action[3];

            // Applp actions to vector
            if (dirToGoForwardAction == 1)
                dirToGo = (largeGrounded ? 1f : 0.5f) * 1f * transform.forward;

            else if (dirToGoForwardAction == 2)
                dirToGo = (largeGrounded ? 1f : 0.5f) * -1f * transform.forward;

            if (rotateDirAction == 1)
                rotateDir = transform.up * -1f;

            else if (rotateDirAction == 2)
                rotateDir = transform.up * 1f;

            if (dirToGoSideAction == 1)
                dirToGo = (largeGrounded ? 1f : 0.5f) * -0.6f * transform.right;

            else if (dirToGoSideAction == 2)
                dirToGo = (largeGrounded ? 1f : 0.5f) * 0.6f * transform.right;

            if (jumpAction == 1) {
                if ((jumpingTime <= 0f) && smallGrounded) {
                    Jump();
                }
            }

            transform.Rotate(rotateDir, Time.fixedDeltaTime * 300f);
            m_AgentRb.AddForce(dirToGo * m_WallJumpSettings.agentRunSpeed, ForceMode.VelocityChange);

            if (jumpTime > 0f) {
                m_JumpTargetPos = new Vector3(
                    m_AgentRb.position.x,
                    m_JumpStartingPos.y + m_WallJumpSettings.agentJumpHeight,
                    m_AgentRb.position.z) + dirToGo;

                MoveTowards(m_JumpTargetPos, m_AgentRb, m_WallJumpSettings.agentJumpVelocity, m_WallJumpSettings.agentJumpVelocityMaxChange);
            }

            if (!(jumpingTime > 0f) && !largeGrounded) {
                m_AgentRb.AddForce(Vector3.down * fallingForce, ForceMode.Acceleration);
            }

            jumpingTime -= Time.fixedDeltaTime;
        }

        // Reset block somewhere within spawn area. 
        void ResetBlock(Rigidbody blockRb) {
            blockRb.transform.position = GetRandomSpawnPos();
            blockRb.velocity = Vector3.zero;
            blockRb.angularVelocity = Vector3.zero;
        }

        // Compute Vector3 which is a random location within spawn area. 
        Vector3 GetRandomSpawnPos() {
            var RandomPosX = Random.Range(-m_SpawnAreaBounds.extents.x, m_SpawnAreaBounds.extents.x);
            var RandomPosZ = Random.Range(-m_SpawnAreaBounds.extents.z, m_SpawnAreaBounds.extents.z);

            var randomSpawnPos = spawnArea.transform.position + new Vector3(
                RandomPosX, 0.45f, RandomPosZ);

            return randomSpawnPos;
        }

        /// <summary>
        /// Move the Rb to a target position smoothly at some speed. 
        /// </summary>
        /// <param name="targetPos">Move to here.</param>
        /// <param name="rb">Body to be moved.</param>
        /// <param name="targetVel">Velocity</param>
        /// <param name="maxVel"></param>
        void MoveTowards(Vector3 targetPos, Rigidbody rb, float targetVel, float maxVel) {
            var moveToPos = targetPos - rb.worldCenterOfMass;
            var velocityTarget = Time.fixedDeltaTime * targetVel * moveToPos;

            if (float.IsNaN(velocityTarget.x) == false) {
                rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, maxVel);
            }
        }

        /// <summary>
        /// Configure agent for wall height. Sets wall height
        /// and appropriate brain.
        /// </summary>
        /// <param name="config">
        /// 0 - No wall, 1 - Small wall, else large wall.
        /// </param>
        void ConfigureAgent(int config) {

            var localScale = wall.transform.localScale;
            if (config == 0) {

                localScale = new Vector3(
                    localScale.x,
                    Academy.Instance.FloatProperties.GetPropertyWithDefault("no_wall_height", 0),
                    localScale.z);

                wall.transform.localScale = localScale;

                GiveModel("SmallWallJump", noWallBrain); // This is from their code. Typo? TODO check.
            }

            else if (config == 1) {
                localScale = new Vector3(
                    localScale.x,
                    Academy.Instance.FloatProperties.GetPropertyWithDefault("small_wall_height", 4),
                    localScale.z);

                wall.transform.localScale = localScale;

                GiveModel("SmallWallJump", smallWallBrain);
            }

            else {
                var min = Academy.Instance.FloatProperties.GetPropertyWithDefault("big_wall_min_height", 8);
                var max = Academy.Instance.FloatProperties.GetPropertyWithDefault("big_wall_max_height", 8);
                var height = min + Random.value * (max - min);
                localScale = new Vector3(
                    localScale.x,
                    Academy.Instance.FloatProperties.GetPropertyWithDefault("small_wall_height", 4),
                    localScale.z);

                wall.transform.localScale = localScale;

                GiveModel("BigWallJump", bigWallBrain);
            }
        }

        public void Jump() {
            jumpingTime = 0.2f;
            m_JumpStartingPos = m_AgentRb.position;
        }

        /// <summary>
        /// Check if agent is grounded by a small big check.
        /// </summary>
        /// <param name="smallCheck">What type of check to make</param>
        /// <returns>True if agent is on the ground, false otherwise.</returns>
        bool DoGroundCheck(bool smallCheck) {

            if (!smallCheck) {
                hitGroundColliders = new Collider[3];
                var obj = gameObject;

                Physics.OverlapBoxNonAlloc(
                    obj.transform.position + new Vector3(0, -0.05f, 0),
                    new Vector3(0.95f / 2f, 0.5f, 0.95f / 2f),
                    hitGroundColliders,
                    obj.transform.rotation);

                var grounded = false;

                foreach (var collision in hitGroundColliders) {
                    if (collision != null && collision.transform != transform &&
                        (collision.CompareTag("walkableSurface") ||
                         collision.CompareTag("block") ||
                         collision.CompareTag("wall"))) {
                        // Agent is grounded.
                        grounded = true; 
                        break;
                    }
                }
                return grounded;
            }
            // Do a small check
            else {
                RaycastHit hit;
                Physics.Raycast(transform.position + new Vector3(0, -0.0f, 0), -Vector3.up, out hit, 1f);

                if (hit.collider != null && (
                    hit.collider.CompareTag("walkableSurface") ||
                    hit.collider.CompareTag("block") ||
                    hit.collider.CompareTag("wall"))
                    && hit.normal.y > 0.95) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        // Agent reached goal.
        private void OnTriggerStay(Collider collider) {
            if (collider.gameObject.CompareTag("goal") && DoGroundCheck(true)) {
                SetReward(1f);
                Done();
                StartCoroutine(
                    GoalScoredSwapGroundMaterial(m_WallJumpSettings.goalMetMaterial, 2));
            }
        }

        /// <summary>
        /// Chenges the color of the ground for a moment
        /// </summary>
        /// <returns>The Enumerator to be used in a Coroutine</returns>
        /// <param name="mat">The material to be swaped.</param>
        /// <param name="time">The time the material will remain.</param>
        IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time) {
            m_GroundRenderer.material = mat;
            yield return new WaitForSeconds(time); //wait for 2 sec
            m_GroundRenderer.material = m_GroundMaterial;
        }

        void FixedUpdate() {
            if (m_Configuration != -1) {
                ConfigureAgent(m_Configuration);
                m_Configuration = -1;
            }
        }
    }
}