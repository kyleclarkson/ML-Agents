using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCollectorPlayer : FoodCollectorAgent {

    /// <summary>
    /// Input recieived is WADS and SPACE from keyboard.
    /// </summary>
    /// <returns></returns>
    public override float[] Heuristic() {
        var actionVector = new float[4];

        if (Input.GetKey(KeyCode.A)) {
            actionVector[2] = 1f;
        }
        if (Input.GetKey(KeyCode.D)) {
            actionVector[2] = 2f;
        }
        if (Input.GetKey(KeyCode.W)) {
            actionVector[0] = 1f;
        }
        if (Input.GetKey(KeyCode.S)) {
            actionVector[0] = 2f;
        }
        actionVector[3] = Input.GetKey(KeyCode.Space) ? 1f : 0f;

        return actionVector;
    }
}
