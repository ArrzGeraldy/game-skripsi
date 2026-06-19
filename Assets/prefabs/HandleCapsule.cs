using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCapsule : MonoBehaviour
{
    public TestPukulNewtonAyunan parent;

    public bool isCollided = false;

 
    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Obstacle") && !isCollided)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if(rb != null)
            {
                Vector3 dir = transform.forward;

                rb.AddForce(dir * parent.currentForce, ForceMode.Impulse);
                Debug.Log(parent.currentForce);
                float ratio = parent.mass / (parent.mass + rb.mass);
                parent.velo *= -ratio;

            }
            isCollided = true;

            StartCoroutine(ExitCollider());
        }
    }

    IEnumerator ExitCollider()
    {
        yield return new WaitForSeconds(1f);
        isCollided = false;
        yield return null;
    }



}
