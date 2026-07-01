using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PuzzleKinematicSix : MonoBehaviour, PuzzleHandlerI
{
    public float targetAnswerv0;
    public float targetAnswerAngle;
    public ParabolaUI inputUI;
    public Transform wall;

    public void CheckAnswer()
    {
        Debug.Log($"vo: {inputUI.v0Slider.value}, angle: {inputUI.angleSlider.value}");
        Debug.Log($"Target vo: {targetAnswerv0}, angle: {targetAnswerv0}");
        if(Mathf.Approximately(targetAnswerv0, inputUI.v0Slider.value) && Mathf.Approximately(targetAnswerAngle, inputUI.angleSlider.value))
        {
            StartCoroutine(CorrectAnswer());
            GameLevelManager.Instance.IncreasePuzzle();

        }
        else
        {
            float deltaV0 = inputUI.v0Slider.value - targetAnswerv0;
            float deltaAngle = inputUI.angleSlider.value - targetAnswerAngle;
            // Panggil otomatis mendeteksi overload 2 parameter
            string msg = FuzzyHint.GetHint(deltaV0,20f, "V0");
            string msg2 = FuzzyHint.GetHint(deltaAngle, 60f, "Sudut");
            inputUI.wrongLabel.text = msg + "\n" + msg2;
            StartCoroutine(WrongAnswer());
            Player.Instance.LoseLife("puzzle 6");

        }
    }

    public IEnumerator CorrectAnswer()
    {
        // wall.DORotate(new Vector3(90, 0, 0), 2);
        wall.DOScale(new Vector3(0, 0, 0), 2);
        inputUI.ShowAlert(AlertType.Success, true);
        yield return new WaitForSeconds(3f);
        Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);
        inputUI.Hide();
        
    }

    public void OnPuzzleActivated()
    {
        Debug.Log("activate puzzle 6");

        inputUI.OnTimerReachedTarget += CheckAnswer;
    }

    public void OnPuzzleDeactivated()
    {
        Debug.Log("activate puzzle 6");

        inputUI.OnTimerReachedTarget -= CheckAnswer;
    }

    public void OnStartSimulation()
    {
    }

    public IEnumerator WrongAnswer()
    {
        inputUI.ShowAlert(AlertType.Wrong, true);
        yield return new WaitForSeconds(1.5f);
        inputUI.ShowAlert(AlertType.Wrong, false);
    }
}
