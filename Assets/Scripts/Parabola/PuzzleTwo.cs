using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTwo : MonoBehaviour, IPuzzleHandler
{
     public Canvas InputParabola;
    [SerializeField]ParabolaUI parabolaUI;
    public float targetLength;
    [SerializeField] float targetV0;
    [SerializeField] float tTarget;
    public Canon canon;
    public Transform targetObs;

    void Start()
    {
        parabolaUI = InputParabola.GetComponent<ParabolaUI>();
        if(parabolaUI == null)
        {
            Debug.LogError("PARABOLA UI NULL");
            return;
        }

        // t = v0/g
        // g = 10m/s^2
        tTarget = targetV0/10f;
        // parabolaUI.tTarget = tTarget *2;
        // Debug.Log("target edit: " + parabolaUI.tTarget);
        parabolaUI.OnTimerReachedTarget += CheckPlayerAnswer;


    }

    void CheckPlayerAnswer()
    {

        if (Mathf.Approximately(parabolaUI.v0Slider.value, targetV0))
        {
            Debug.Log("BENARR !!; DEGAN VELO: " + parabolaUI.v0Slider.value);
            StartCoroutine(CorrectAnswer());
            
        }
        else
        {
            Debug.Log("SALAHHH !!; DEGAN VELO: " + parabolaUI.v0Slider.value);
            Debug.Log("SALAHHH !!; target v0: " + targetV0);
            string hint = parabolaUI.v0Slider.value > targetV0 ? "besar" : "kecil";
            string msg = "v<sub>0</sub> terlalu " + hint;
            parabolaUI.wrongLabel.text = msg;
            StartCoroutine(WrongAnswer());


        }

        canon.DestroyBullet();
    }

    IEnumerator CorrectAnswer()
    {
        parabolaUI.ShowAlert(AlertType.Success, true);

        float duration = 1f;
        float timeElapsed = 0f;

        Quaternion startRot = targetObs.localRotation;
        Quaternion endRot = Quaternion.Euler(0, 0, 90);
        
        Vector3 startPos = targetObs.localPosition;
        Vector3 targetPos = new Vector3(0, -0.75f, targetObs.localPosition.z);

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            float t  = timeElapsed / duration;
            
            targetObs.localRotation = Quaternion.Slerp(startRot, endRot, t);
            targetObs.localPosition = Vector3.Lerp(startPos, targetPos, t);
            
            yield return null; 
        }


        yield return new WaitForSeconds(1.5f);
        Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);
        parabolaUI.Hide();
        yield return null;
    }

    IEnumerator WrongAnswer()
    {
        parabolaUI.ShowAlert(AlertType.Wrong, true);

        yield return new WaitForSeconds(2f);
        parabolaUI.ShowAlert(AlertType.Wrong, false);

        yield return null;
    }

    void OnDestroy()
    {
        if (parabolaUI != null)
        {
            parabolaUI.OnTimerReachedTarget -= CheckPlayerAnswer;
        }
    }

    public void OnPuzzleActivated()
    {
        parabolaUI.OnTimerReachedTarget += CheckPlayerAnswer;
    }

    public void OnPuzzleDeactivated()
    {
        parabolaUI.OnTimerReachedTarget -= CheckPlayerAnswer;
    }
}
