using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Redone {

    public class PushBlockSettings : MonoBehaviour {

        // Movement speed of agent.
        public float agentRunSpeed;

        // Rotation speed.
        public float agentRotationSpeed;

        // Fractional amount in [0,1] corresponding 
        // to the precentage of the spwan area that is used.
        public float spawnAreaMarginMultiplier;

        // Ground material when goal is scored.
        public Material goalScoredMaterial;

        // Ground material when agent fails. 
        public Material failMaterial;
    }
}
