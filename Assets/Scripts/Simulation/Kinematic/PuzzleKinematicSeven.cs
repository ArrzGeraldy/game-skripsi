using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PuzzleKinematicSeven : MonoBehaviour, PuzzleHandlerI
{
    public float targetAnswerV0;
    public float targetAnswerAngle;
    public ParabolaUI inputUI;
    public Transform sphere;
    public Ease ease;

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
            string msg = FuzzyHint.GetHint(deltaV0,20f, "V0");
            string msg2 = FuzzyHint.GetHint(deltaAngle, 60f, "Sudut");
            inputUI.wrongLabel.text = msg + "\n" + msg2;
            StartCoroutine(WrongAnswer());
            Player.Instance.LoseLife("puzzle 7");

        }
    }


    public IEnumerator CorrectAnswer()
    {
        sphere.DOScale(0,3f).SetEase(ease);
        inputUI.ShowAlert(AlertType.Success, true);
        yield return new WaitForSeconds(5.5f);
        GameLevelManager.Instance.IncreasePuzzle();

        Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);
        inputUI.Hide();
        
    }

    public void OnPuzzleActivated()
    {
        Debug.Log("activate puzzle 7");

        inputUI.OnTimerReachedTarget += CheckAnswer;
    }

    public void OnPuzzleDeactivated()
    {
        Debug.Log("activate puzzle 7");

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
