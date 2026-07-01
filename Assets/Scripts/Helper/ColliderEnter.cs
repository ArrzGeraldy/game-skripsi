using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderEnter : MonoBehaviour
{
    public UnityEvent onPlayerEnter;
    bool hasCollided;
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider col)
    {
        Debug.Log("collider enter: " + col.tag);
        if(col.CompareTag("Player") && !hasCollided)
        {
            Debug.Log("Player Enter");
            onPlayerEnter?.Invoke();
            hasCollided = true;
        }
    }

  
}
