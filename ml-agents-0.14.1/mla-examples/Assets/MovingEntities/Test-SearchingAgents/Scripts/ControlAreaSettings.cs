using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class ControlAreaSettings : MonoBehaviour {

    public ControlAreaSmall area;

    public void Awake() {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset; 
    }

    public void EnvironmentReset() {
        Debug.Log("Env reset called");
        area = FindObjectOfType<ControlAreaSmall>();
        area.ResetArea();
    }
}
