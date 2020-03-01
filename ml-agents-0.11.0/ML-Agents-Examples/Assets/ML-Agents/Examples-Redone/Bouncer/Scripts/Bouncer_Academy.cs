using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Bouncer_Academy : Academy {

    public float gravityMultiplier = 1f;

    public override void AcademyReset() {
        Physics.gravity = new Vector3(0, -9.8f * gravityMultiplier, 0);
    }

    public override void AcademyStep() {
        Debug.Log("AcadempyStep() called");
    }

    public override void InitializeAcademy() {
        Debug.Log("InitializeAcademy() called");
    }
}
