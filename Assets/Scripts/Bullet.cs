using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 initPos;
    TrailRenderer trail;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initPos = transform.position;
        
        // Tambah TrailRenderer via code
        trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 4f;          
        trail.startWidth = 0.15f;
        trail.endWidth = 0.1f; 
        
        // Warna jejak
        trail.startColor = Color.yellow;
        trail.endColor = new Color(1f, 1f, 0f, 0f); // fade out
        
        
        trail.material = new Material(Shader.Find("Sprites/Default"));
        StartCoroutine(DestoryBullet());
    }

    IEnumerator DestoryBullet()
    {
        yield return new WaitForSeconds(3.5f);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.transform.CompareTag("Key"))
        {
            Debug.Log("Key hitted");
        }
        // Destroy(gameObject);
    }

    void Update()
    {
    }
}