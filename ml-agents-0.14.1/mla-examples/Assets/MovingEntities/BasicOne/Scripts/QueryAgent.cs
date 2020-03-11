using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class QueryAgent : Agent {

    public Rigidbody m_AgentRb;
    public GameObject area;
    public QueryPointSettings m_QueryPointSettings;

    // Agent movement properties.
    public float turnSpeed = 300;
    public float moveSpeed = 2;


    public override void AgentAction(float[] vectorAction) {
        base.AgentAction(vectorAction);
    }

    public override void AgentReset() {
        base.AgentReset();
    }

    public override void CollectObservations() {
        base.CollectObservations();
    }

    public override float[] Heuristic() {
        ///

        ///
        var action = new float[4];

    }

    public override void InitializeAgent() {
        base.InitializeAgent();
    }
}
