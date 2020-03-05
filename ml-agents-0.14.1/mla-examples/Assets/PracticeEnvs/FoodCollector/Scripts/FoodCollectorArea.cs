using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAgentsExamples;

namespace Examples {

    public class FoodCollectorArea : Area {

        public GameObject food;
        public GameObject badFood;

        public int numFood;
        public int numBadFood;

        public bool respawnFood;
        public float range;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num">The amount of food to create</param>
        /// <param name="foodItem">The food object to be cloned</param>
        public void CreateFood(int numOfFood, GameObject foodItem) {

            for (int i=0; i < numOfFood; i++) {
                GameObject food = Instantiate(foodItem, new Vector3(
                    Random.Range(-range, range), 1f, Random.Range(-range, range)) + transform.position,
                    Quaternion.Euler(new Vector3(0f, Random.Range(0f,360f), 90f)));

                food.GetComponent<FoodLogic>().respawn = respawnFood;
                food.GetComponent<FoodLogic>().myArea = this;
            }
        }


        public void ResetFoodArea(GameObject agent) {
            // TODO
        }

        public override void ResetArea() {
            // Empty override method.
        }
    }
}
