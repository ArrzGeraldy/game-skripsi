using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class PuzzleKinematicThree : MonoBehaviour, PuzzleHandlerI
{
    public Light pointLight;
    public float targetAnswer;
    public ParabolaUI inputUI;
    public Transform stairs;

    void Start()
    {
    }

    public void CheckAnswer()
    {
        if (Mathf.Approximately(inputUI.v0Slider.value, targetAnswer))
        {
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
        pointLight.color = Color.green;
        stairs.DOLocalMoveY(1.844455f, 2f);

        inputUI.ShowAlert(AlertType.Success, true);
        
        yield return new WaitForSeconds(3f);
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
        pointLight.color = Color.red;
        StartCoroutine(ChangeColor());

    }

    IEnumerator ChangeColor()
    {
        while(inputUI.inSimulation)
        {

            pointLight.color = Color.Lerp(Color.red, Color.blue,inputUI.CurrTime);
            yield return null;
        }
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
