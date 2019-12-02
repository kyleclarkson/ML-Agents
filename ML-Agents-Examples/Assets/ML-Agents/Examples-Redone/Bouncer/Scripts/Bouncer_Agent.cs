using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Bouncer_Agent : Agent {

    [Header("Bouncer Specific")]
    public GameObject target;
    public GameObject bodyObject;

    Rigidbody m_Rigidbody;
    Vector3 m_LookingDirection;

    public float strength = 10f;
    float m_JumpCoolDown;
    int m_NumberOfJumps = 20;
    int m_JumpsLeft = 20;

    ResetParameters m_ResetParams;

    public override void InitializeAgent() {
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        m_LookingDirection = Vector3.zero;

        var academy = FindObjectOfType<Academy>();
        m_ResetParams = academy.resetParameters;

        SetResetParameters();
    }

    public override float[] Heuristic() {
        // Actions are a size 3 vector for horizontal,jumping, and vertical motion. 
        var action = new float[3];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
        action[2] = Input.GetAxis("Vertical");

        return action;
    }

    public override void CollectObservations() {
        // Position of agent.
        AddVectorObs(gameObject.transform.localPosition);
        // Position of target. 
        AddVectorObs(target.transform.position);
    }


    public override void AgentAction(float[] vectorAction, string textAction) {
        
        // Ensure action-values are clamped to [-1,1]
        for (var i=0; i<vectorAction.Length; i++) {
            vectorAction[i] = Mathf.Clamp(vectorAction[i], -1, 1);
        }

        // Get horizontal, jump, and vertical action-values.
        var x = vectorAction[0];
        var y = ScaleAction(vectorAction[1], 0, 1);
        var z = vectorAction[2];
        // Apply a force to agent given the actions.
        m_Rigidbody.AddForce(new Vector3(x, y + 1, z) * strength);

        // Observe reward.
        // Currently proportional to magnitude of action.
        AddReward(-0.05f * (
            vectorAction[0] * vectorAction[0] +
            vectorAction[1] * vectorAction[1] +
            vectorAction[2] * vectorAction[2]) / 3f);

        // Update direction of view. 
        m_LookingDirection = new Vector3(x, y, z);
    }

    public override void AgentOnDone() {
        // Empty.
        Debug.Log("AgentOnDone() called.");
    }

    public override void AgentReset() {
        gameObject.transform.localPosition = new Vector3(
            (1 - 2 * Random.value) * 5,
            2,
            (1 - 2 * Random.value) * 5);

        m_Rigidbody.velocity = default(Vector3);
        var environment = gameObject.transform.parent.gameObject;
        var targets = environment.GetComponentsInChildren<Bouncer_Target>();

        foreach (var target in targets) {
            target.Respawn();
        }
        m_JumpsLeft = m_NumberOfJumps;

        SetResetParameters();

    }

    /*
      ==== Private Functions ====
        */

    private void SetResetParameters() {
        SetTargetScale();
    }

    private void SetTargetScale() {
        var targetScale = m_ResetParams["target_scale"];
        target.transform.localScale = new Vector3(targetScale, targetScale, targetScale);
    }

    // Note: Update runs once per frame, while FixedUpdate runs when needed (0, or multiple times per frame.) Thus the latter should beused when applying physics. 
    private void Update() {
        if (m_LookingDirection.magnitude > float.Epsilon) {
            // Rotate object from rotation to LookingDirection over deltaTime*10 seconds. 
            bodyObject.transform.rotation = Quaternion.Lerp(
                bodyObject.transform.rotation,
                Quaternion.LookRotation(m_LookingDirection),
                Time.deltaTime * 10f);
        }
    }

    //TODO 
    private void FixedUpdate() {
        
    }
}
