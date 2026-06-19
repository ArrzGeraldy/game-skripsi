using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPadController : MonoBehaviour
{
    public Vector3 force = new Vector3(0, 40f, 40f);
    public bool onCollided = false;

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player") && !onCollided)
        {
            onCollided = true;
            Player.Instance.AddForce(force);
        }
    }
}
