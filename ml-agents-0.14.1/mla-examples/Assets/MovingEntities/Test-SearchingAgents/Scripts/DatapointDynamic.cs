using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class DatapointDynamic : MonoBehaviour {

    public PathCreator pathCreator;
    public float speed = 5;
    float dstTravelled;

    // Update is called once per frame
    void Update()
    {
        dstTravelled += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(dstTravelled);
        transform.rotation = pathCreator.path.GetRotationAtDistance(dstTravelled);
    }
}
