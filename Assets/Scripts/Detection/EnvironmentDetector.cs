// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public struct HitDataObstacle
// {
//     public bool forwardHitFound;
//     public RaycastHit forwardHit;
//     public bool heightHitFound;
//     public RaycastHit heightHit;

// }

// public class EnvironmentDetector : MonoBehaviour
// {
//      public Vector3 forwardRayOffset = new Vector3(0, 0.25f, 0);
//     [SerializeField] float forwardRayLength = 0.8f;
//     [SerializeField] float heightRayLength = 5.0f;
//     [SerializeField] LayerMask obstacleLayerMask;

//     public HitDataObstacle ObstacleCheck()
//     {
//         HitDataObstacle data = new HitDataObstacle();

//         var origin = transform.position + forwardRayOffset;

//         data.forwardHitFound = Physics.Raycast(origin, transform.forward, out data.forwardHit, forwardRayLength, obstacleLayerMask);

//         Debug.DrawRay(origin, transform.forward * forwardRayLength,   data.forwardHitFound ? Color.red : Color.white);

//         if(data.forwardHitFound)
//         {
//             var heightOrigin = data.forwardHit.point + Vector3.up * heightRayLength;
//             data.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out data.heightHit, heightRayLength, obstacleLayerMask);

//             Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength,   data.heightHitFound ? Color.red : Color.white);
//         }

//         return data;

//     }
// }
