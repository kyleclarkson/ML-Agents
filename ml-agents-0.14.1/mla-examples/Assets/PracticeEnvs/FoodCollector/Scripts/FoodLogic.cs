using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples {

    public class FoodLogic : MonoBehaviour {

        public bool respawn;
        public FoodCollectorArea myArea;

        public void OnEaten() {

            // Food item will either respawn or not. 
            if (respawn) {
                transform.position = new Vector3(
                    Random.RandomRange(-myArea.range, myArea.range),
                    3f,
                    Random.RandomRange(-myArea.range, myArea.range) + myArea.transform.position;
            } else {
                Destroy(gameObject);
            }
        }
    }

}
