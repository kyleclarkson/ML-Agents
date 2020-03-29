using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class QueryPointSettings : MonoBehaviour {

    [HideInInspector]
    public GameObject[] agents;
    [HideInInspector]
    public QueryPointArea[] listArea;

    public int totalScore;
    public Text scoreText;

    public void Awake() {
        Debug.Log("Awake called");
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
    }

    public void EnvironmentReset() {
        Debug.Log("Env reset");

        // TODO clear previous object

        // Set up areas
        agents = GameObject.FindGameObjectsWithTag("agent");
        listArea = FindObjectsOfType<QueryPointArea>();
        Debug.Log("Areas: " + listArea.Length);
        foreach (var queryArea in listArea) {
            queryArea.ResetQueryArea();
        }
    }
}
