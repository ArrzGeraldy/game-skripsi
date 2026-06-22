using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleKinematicSeven : MonoBehaviour, PuzzleHandlerI
{
    public float targetAnswerV0;
    public float targetAnswerAngle;
    public ParabolaUI inputUI;

    public void CheckAnswer()
    {

        if (Mathf.Approximately(inputUI.v0Slider.value, targetAnswerV0) && Mathf.Approximately(inputUI.angleSlider.value, targetAnswerAngle))
        {
            StartCoroutine(CorrectAnswer());

            Debug.Log("BENARR !!; DEGAN VELO: " + inputUI.v0Slider.value);
            
        }
        else
        {
            float deltaV0 = inputUI.v0Slider.value - targetAnswerV0;
            float deltaAngle = inputUI.angleSlider.value - targetAnswerAngle;
            // Panggil otomatis mendeteksi overload 2 parameter
            string hint = HintSystem.RunFuzzyEngine(deltaAngle, deltaV0); 
            inputUI.wrongLabel.text = hint;
            StartCoroutine(WrongAnswer());
            Player.Instance.LoseLife();

        }
    }


    public IEnumerator CorrectAnswer()
    {
        inputUI.ShowAlert(AlertType.Success, true);
        yield return new WaitForSeconds(4.5f);
        Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);
        inputUI.Hide();
        
    }

    public void OnPuzzleActivated()
    {
        inputUI.OnTimerReachedTarget += CheckAnswer;
    }

    public void OnPuzzleDeactivated()
    {
        inputUI.OnTimerReachedTarget -= CheckAnswer;
    }

    public void OnStartSimulation()
    {
    }

    public IEnumerator WrongAnswer()
    {
        inputUI.ShowAlert(AlertType.Wrong, true);
        yield return new WaitForSeconds(4.5f);
        inputUI.ShowAlert(AlertType.Wrong, false);
    }
}
