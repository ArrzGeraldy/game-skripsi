using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleNewtonOne : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public float mass = 2f;
    public float dynamicFriction = 0.4f;
    public float inputForce = 0f;

    private Rigidbody rb;
    private bool isInZone = false;
    private bool hasApplied = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;

        // Setup friction
        PhysicMaterial mat = GetComponent<Collider>().material;
        mat.dynamicFriction = dynamicFriction;
        mat.staticFriction = dynamicFriction + 0.2f;
    }

    // Dipanggil dari tombol UI "Apply Force"
    public void ApplyForce()
    {
        rb.AddForce(Vector3.forward * inputForce, ForceMode.Impulse);
        hasApplied = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            ApplyForce();
        }
    }

    // Deteksi masuk SimulationZone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SimulationZone"))
            isInZone = true;

        // Cek apakah sampai GoalZone
        if (other.CompareTag("GoalZone"))
            Debug.Log("SUKSES! Kotak sampai di tujuan!");
    }
}
