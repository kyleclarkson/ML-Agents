using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class FoodCollectorAcademy : Academy
{
    [HideInInspector]
    public GameObject[] agents;

    [HideInInspector]
    public FoodCollectorArea[] listArea;

    public int totalScore;
    public Text scoreText;

    /// <summary>
    /// Reset areas. 
    /// </summary>
    public override void AcademyReset() {
        ClearObjects(GameObject.FindGameObjectsWithTag("food"));
        ClearObjects(GameObject.FindGameObjectsWithTag("badFood"));

        agents = GameObject.FindGameObjectsWithTag("agent");
        listArea = FindObjectsOfType<FoodCollectorArea>();
        // Reset each area.
        foreach (var area in listArea) {
            area.ResetFoodArea(agents);
        }

        totalScore = 0;
    }

    /// <summary>
    /// Update score display.
    /// </summary>
    public override void AcademyStep() {
        scoreText.text = string.Format(@"Score: {0}", totalScore);
    }

    /// <summary>
    /// Remove any food items present.
    /// </summary>
    /// <param name="objects">Objects to be removed.</param>
    void ClearObjects(GameObject[] objects) {
        foreach(var food in objects) {
            Destroy(food);
        }
    }
}
