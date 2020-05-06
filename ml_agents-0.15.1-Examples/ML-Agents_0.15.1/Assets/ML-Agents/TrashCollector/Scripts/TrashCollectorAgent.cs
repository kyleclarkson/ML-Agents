using UnityEngine;
using MLAgents;

public class TrashCollectorAgent : Agent {

    TrashCollectorSettings m_TrashCollectorSettings;
    Rigidbody m_AgnetRb;
    public float m_TurnSpeed = 300;
    public float m_MoveSpeed = 3;

    public override void Initialize() {
        m_AgnetRb = GetComponent<Rigidbody>();
        m_TrashCollectorSettings = FindObjectOfType<TrashCollectorSettings>();


    }

}
