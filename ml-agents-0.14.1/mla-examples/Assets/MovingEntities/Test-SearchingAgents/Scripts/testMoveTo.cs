using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class testMoveTo : MonoBehaviour
{

    public Transform target;
    public Rigidbody m_Rb;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        m_Rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        m_Rb.transform.position = Vector3.MoveTowards(m_Rb.transform.position, target.position, step);

    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("target")) {
            Debug.Log("target reached");
        }
    }
}
