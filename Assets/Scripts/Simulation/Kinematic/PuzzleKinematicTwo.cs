using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleKinematicTwo : MonoBehaviour, PuzzleHandlerI
{
    public float targetAnswer;
    public ParabolaUI inputUI;
    public Light pointLight;
    public Transform doors;

    void Start()
    {
        if(pointLight)
            pointLight.color = Color.red;    

        SetDoorsFrozen(true);
    }

    void Update()
    {
        
    }

    public void CheckAnswer()
    {

        pointLight.color = Color.green;
        // StartCoroutine(dangerLight());
        SetDoorsFrozen(false);

        if (Mathf.Approximately(inputUI.v0Slider.value, targetAnswer))
        {
            pointLight.color = Color.green;
            DoorsAddForce();
            StartCoroutine(CorrectAnswer());


            Debug.Log("BENARR !!; DEGAN VELO: " + inputUI.v0Slider.value);
            
        }
        else
        {
            pointLight.color = Color.red;
            Debug.Log("SALAHHH !!; DEGAN VELO: " + inputUI.v0Slider.value);
            string hint = inputUI.v0Slider.value > targetAnswer ? "besar" : "kecil";
            string msg = "v<sub>0</sub> terlalu " + hint;
            inputUI.wrongLabel.text = msg;
            StartCoroutine(WrongAnswer());
        }
    }

    public IEnumerator CorrectAnswer()
    {
        inputUI.ShowAlert(AlertType.Success, true);
        
        yield return new WaitForSeconds(1.5f);
        Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);
        inputUI.Hide();
        yield return null;
    }

    public IEnumerator WrongAnswer()
    {
        inputUI.ShowAlert(AlertType.Wrong, true);

        yield return new WaitForSeconds(2f);
        inputUI.ShowAlert(AlertType.Wrong, false);

        yield return null;
    }

    public void OnPuzzleActivated()
    {
        inputUI.OnTimerReachedTarget += CheckAnswer;
        inputUI.OnStartSimulation += OnStartSimulation;
    }

    public void OnPuzzleDeactivated()
    {
        inputUI.OnTimerReachedTarget -= CheckAnswer;
        inputUI.OnStartSimulation -= OnStartSimulation;
    }

    public void OnStartSimulation()
    {
        pointLight.color = Color.blue;
        SetDoorsFrozen(true);
    }

    IEnumerator dangerLight()
    {
        yield return new WaitForSeconds(1f);
        pointLight.color = Color.red;
    }

    void SetDoorsFrozen(bool val)
    {
        foreach(Transform door in doors)
        {
            Rigidbody rb = door.GetComponent<Rigidbody>();
            if(rb)
                rb.isKinematic = val;
        }
    }

    void DoorsAddForce()
    {
        foreach(Transform door in doors)
        {
            Rigidbody rb = door.GetComponent<Rigidbody>();
            if(rb)
            {
                rb.isKinematic = false;
                rb.AddForce(new Vector3(0, 0, 50f));
            }
        }
    }
}
