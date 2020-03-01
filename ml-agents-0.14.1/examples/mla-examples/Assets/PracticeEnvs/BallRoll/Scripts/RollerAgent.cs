using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAgents;
using MLAgents.Sensor;



public class RollerAgent : Agent
{

    Rigidbody rBody;
    public Transform TargetBall;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void AgentReset() {
        if(this.transform.position.y < 0) {
            // Agent has fallen off plane. Reset agent.
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0, 0.5f, 0);
        }

        // Target is reached. Reset its location. 
        TargetBall.position = new Vector3(
            Random.value * 8 - 4,
            0.5f,
            Random.value * 8 - 4);
    }
}
