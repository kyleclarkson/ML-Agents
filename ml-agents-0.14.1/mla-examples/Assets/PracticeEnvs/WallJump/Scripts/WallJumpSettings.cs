using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples {

    public class WallJumpSettings : MonoBehaviour {

        [Header("Sepcific to WallJump")]
        public float agentRunSpeed;
        public float agentJumpHeight;

        // Ground will change color if goal is met/failed.
        public Material goalMetMaterial;
        public Material goalFailedMaterial;

        [HideInInspector]
        public float agentJumpVelocity = 777;
        [HideInInspector]
        public float agentJumpVelocityMaxChange = 10;
    }
}
