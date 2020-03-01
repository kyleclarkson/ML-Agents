using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Bouncer_Target : MonoBehaviour {
    // Update is called once per frame
    void FixedUpdate() {
        // Rotate target for display.
        gameObject.transform.Rotate(new Vector3(1, 0, 0), 0.5f);
    }

    /// <summary>
    /// Called when BouncerAgent makes contact with target. 
    /// </summary>
    /// <param name="collision">The collision event between agent and target</param>
    private void OnTriggerEnter(Collider collision) {
        var agent = collision.gameObject.GetComponent<Agent>();
        if (agent != null) {
            agent.AddReward(1f);
            Respawn();
        }
    }

    public void Respawn() {
        gameObject.transform.localPosition = new Vector3(
            (1 - 2 * Random.value) * 5f,
            2f + Random.value * 5f,
            (1 - 2 * Random.value) * 5f);
    }
}
