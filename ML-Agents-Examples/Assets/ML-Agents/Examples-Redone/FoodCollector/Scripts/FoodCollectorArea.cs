using UnityEngine;
using MLAgents;

public class FoodCollectorArea : Area
{
    public GameObject food;
    public GameObject badFood;
    public int numFood;
    public int numBadFood;
    public bool respawnFood;
    public float range;

    /// <summary>
    /// Creates the number of food items in area.
    /// </summary>
    /// <param name="numOfFoods">Number of food items to spawn</param>
    /// <param name="type"></param>
    void CreateFood(int numOfFoods, GameObject type) {
        GameObject food = Instantiate(
            type,
            new Vector3(Random.RandomRange(-range, range), 1f, Random.Range(-range, range)) + transform.position,
            Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));

        food.GetComponent<FoodLogic>().respawn = respawnFood;
        food.GetComponent<FoodLogic>().myArea = this;
    }

    public void ResetFoodArea(GameObject[] agents) {
        // Place agents in scene.
        foreach (GameObject agent in agents) {

            if(agent.transform.parent == gameObject.transform) {
                agent.transform.position = new Vector3(
                    Random.Range(-range, range),
                    2f,
                    Random.Range(-range, range)) + transform.position;

                agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            }
        }

        // Place food items in scene.
        CreateFood(numFood, food);
        CreateFood(numBadFood, badFood);
    }

    // Override method.
    public override void ResetArea() {

    }
}
