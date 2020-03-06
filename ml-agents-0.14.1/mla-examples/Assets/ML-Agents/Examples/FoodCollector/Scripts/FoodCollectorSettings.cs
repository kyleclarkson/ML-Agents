using System;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

namespace Examples {
    public class FoodCollectorSettings : MonoBehaviour {

        [HideInInspector]
        public GameObject agent;
        [HideInInspector]
        public FoodCollectorArea[] listArea;

        public int totalScore;
        public Text scoreText;

        void Awake() {
            Debug.Log($"FoodCollectorSettings awake called");
            Academy.Instance.OnEnvironmentReset += EnvironmentReset;
        }

        public void EnvironmentReset() {
            ClearObjects(GameObject.FindGameObjectsWithTag("food"));
            ClearObjects(GameObject.FindGameObjectsWithTag("badFood"));

            agent = GameObject.FindGameObjectWithTag("agent");
            listArea = FindObjectsOfType<Examples.FoodCollectorArea>();

            foreach (var foodArea in listArea) {
                foodArea.ResetFoodArea(agent);
            }

            totalScore = 0;
        }

        void ClearObjects(GameObject[] objects) {
            foreach (var food in objects) {
                Destroy(food);
            }
        }

        public void Update() {
            scoreText.text = $"Score: {totalScore}";
        }
    }
}
