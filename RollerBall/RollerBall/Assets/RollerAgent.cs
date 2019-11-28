using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RollerAgent : Agent
{
   
    Rigidbody agentBody;
    // The target cube.
    public Transform target;
    // Agent's speed. 
    public float speed = 10;


    // Start is called before the first frame update
    void Start()
    {
        agentBody = GetComponent<Rigidbody>();
    }

    public override void AgentReset() {
        // Agent has fallen off plane.
        if (this.agentBody.position.y < 0) {
            agentBody.angularVelocity = Vector3.zero;
            agentBody.velocity = Vector3.zero;
            this.agentBody.position = new Vector3(0, 0.5f, 0);
        }

        // Move target.
        target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);

    }

    /*
     Collect observations made by agent. Agent position, Target positon, Agent's velocity.
         
         */
    public override void CollectObservations() {
        AddVectorObs(target.position);
        AddVectorObs(this.transform.position);

        AddVectorObs(agentBody.velocity.x);
        AddVectorObs(agentBody.velocity.z);
    }

    /*
     Recieve actions from brain (after observations considered), apply actions and receive reward. 
         */
    public override void AgentAction(float[] vectorAction, string textAction) {
        // Receive actions from brain. (2D vector - force in x and z direction)
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];

        agentBody.AddForce(controlSignal * speed);

        // Observe reward - distance to target after action taken.
        float dstToTarget = Vector3.Distance(agentBody.position, target.position);

        // Check if target is reached.
        if (dstToTarget < 1.42f) {
            SetReward(1.0f);
            Done();
        }

        if (this.transform.position.y < 0) {
            Done();
        }
    }

    /*
     Allow for kb input for debuging
         */
    public override float[] Heuristic() {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }
}
