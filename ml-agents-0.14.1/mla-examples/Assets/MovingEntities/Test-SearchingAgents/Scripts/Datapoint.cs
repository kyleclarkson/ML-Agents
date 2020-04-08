using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Datapoint : MonoBehaviour {

    public float queryPeriod;
    public float timeLastQueried;
    public float timeSinceLastQueried;
    Renderer renderer;

    // Start is called before the first frame update
    void Start(){
        resetTimes();
        renderer = GetComponent<Renderer>();
    }

    public bool IsOverdue() {
        if (queryPeriod - timeSinceLastQueried < 0) {
            return true;
        }
        return false;
    }

    public float TimeOverdue() {
        return queryPeriod - timeSinceLastQueried;
    }

    void Update() {
        if (IsOverdue()) {
            renderer.material.SetColor("_Color", Color.red);
        } else {
            renderer.material.SetColor("_Color", Color.green);
        }
    }

    public void resetTimes() {
        // Randomly initialize time since last queried 
        timeSinceLastQueried = Random.Range(0, queryPeriod / 2);

        timeLastQueried = 0;
    }
}
