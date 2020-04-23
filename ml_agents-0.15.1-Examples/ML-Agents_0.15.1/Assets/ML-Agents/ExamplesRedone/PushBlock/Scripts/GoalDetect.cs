using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Redone {

    public class GoalDetect : MonoBehaviour {

        // Set by agent script on initialization.
        [HideInInspector]
        public PushAgentBasic agent;

        private void OnCollisionEnter(Collision collision) {
            
            if (collision.gameObject.CompareTag("goal")) {
                agent.ScoredAGoal();
            }
        }
    }
}

