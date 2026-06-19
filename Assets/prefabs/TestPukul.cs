using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPukulNewtonAyunan : MonoBehaviour
{
    public float forceTarik = 15f;
    public float maxTension = 200f;
    public float currentTension;

    public float rotationX;
    public float angularVelocity;

    public float gravityForce = 20f;
    public float damping = 0.5f;
    public float mass = 1f;
    public float velo = 0;
    public float L = 1.5f;
    public float h = 0;
    public float maxVelo = 0f;
    public float maxAngularVelo = 0f;
    public float maxImpactForce = 0f;
    public float currentForce = 0f;
    
    

    public bool swing = false;

    void Update()
    {

        if (Input.GetKey(KeyCode.B) && !swing)
        {
            currentTension += forceTarik * Time.deltaTime;
            currentTension = Mathf.Clamp(currentTension, 0, maxTension);

            float resistanceFactor = 1f - (currentTension/maxTension);

            rotationX += resistanceFactor * forceTarik * Time.deltaTime;
            h = L * (1 - Mathf.Cos(rotationX * Mathf.Deg2Rad));

            maxVelo = Mathf.Sqrt(2 * 9.81f * h);
            maxAngularVelo = maxVelo / L;

            float maxMomentum = mass * maxVelo;

            float impactTime = 0.1f;

            maxImpactForce =
                maxMomentum / impactTime;


        }
        if(Input.GetKeyUp(KeyCode.B))
        {
            swing = true;
        }

        if(swing)
        {
            currentTension -= 50f * Time.deltaTime;
            currentTension = Mathf.Clamp(currentTension, 0, maxTension);


            // --------------  pergerakan bandul -------------- 
            float acceleration = -9.81f/L * Mathf.Sin(rotationX * Mathf.Deg2Rad);

            // simulasi hambatan udara
            // jika velo = 0; maka tidak ada hambatan udara
            // jika velo BESAR; maka hambatan udara juga besar
            acceleration -= damping * velo;

            velo += acceleration * Time.deltaTime;
            rotationX += velo;

            if(Mathf.Abs(velo) < 0.1f && Mathf.Abs(rotationX) < 0.1f)
            {
                velo = 0;
                rotationX = 0;
                currentTension = 0f;
                swing = false;
            }


            // -------------- convert angular velo => linear velo -------------- 

            float linearVelo = Mathf.Abs(velo * L);
            float momentum = mass * linearVelo;
            float impactTime = 0.1f;

            currentForce = momentum/impactTime;
            

            // -------------- pergerakan linear -------------- 
            // f = m * g * sin(theta)
            // float F = mass * -9.81f * Mathf.Sin(rotationX * Mathf.Deg2Rad);

            // float acceleration = F/mass;

            // velo += acceleration * Time.deltaTime;

            // rotationX += velo * Time.deltaTime;


        }

        transform.localRotation =
            Quaternion.Euler(rotationX, 0, 0);
    
    }
}