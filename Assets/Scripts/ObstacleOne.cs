using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleOne : MonoBehaviour
{
    public Transform jembatan;
    public Vector3 targetPos = new Vector3(0, -0.7f, 115.0f);
    private bool isTriggered = false;
    public Canvas InputParabola;
    public Canvas DistanceHint;

    void OnCollisionEnter(Collision col)
    {
        if(col.transform.CompareTag("Bullet") && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(TurunkanJembatan());
            InputParabola.gameObject.SetActive(false);
            DistanceHint.gameObject.SetActive(false);
        }
    }

    IEnumerator TurunkanJembatan()
    {
        while(Vector3.Distance(jembatan.position, targetPos) > 0.01f)
        {
            jembatan.position = Vector3.Lerp(jembatan.position, targetPos, Time.deltaTime);
            jembatan.rotation = Quaternion.Slerp(jembatan.rotation, Quaternion.Euler(90, 0, 0), Time.deltaTime);
            yield return null; // tunggu 1 frame
        }

        jembatan.position = targetPos; // snap ke posisi akhir
            Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);

    }

}
