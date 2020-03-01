using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAgents;
using MLAgents.Sensor;



public class RollerAgent : Agent
{

    Rigidbody rBody;
    public Transform TargetBall;
    public float agentSpeed;

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

    // TODO This may need to be override instead of virutal. 
    public virtual void CollectObservations(VectorSensor sensor) {

        // Add target ball and agent's position
        sensor.AddObservation(TargetBall.position);
        sensor.AddObservation(this.transform.position);

        // x, z velocity of agent
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public override void AgentAction(float[] vectorAction) {

        // 2 Actions: Force in X and Z direction.
        Vector3 control = Vector3.zero;
        control.x = vectorAction[0];
        control.z = vectorAction[1];
        rBody.AddForce(control * agentSpeed);

        // Reward

        float dstToTarget = Vector3.Distance(this.transform.position, TargetBall.position);

        if (dstToTarget < 1.42f) {
            // Close enough to target.
            SetReward(1.0f);
            Done();
        }

        if (this.transform.position.y < 0) {
            // Fell off clift.
            SetReward(-1.0f);
            Done();
        }
    }

}
