

using UnityEngine;


public class GoalDetect_ex : MonoBehaviour {

    public PushAgentBasic_ex agent;

    private void OnCollisionEnter(Collision collision) {

        // Block has entered goal area. 
        if (collision.gameObject.CompareTag("goal")) {
            // TODO
        }
    }
}
