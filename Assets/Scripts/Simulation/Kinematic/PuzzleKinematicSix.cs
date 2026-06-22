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
        }
        else
        {
            inputUI.wrongLabel.text = "SALAH";
            StartCoroutine(WrongAnswer());
            Player.Instance.LoseLife();

        }
    }

    public IEnumerator CorrectAnswer()
    {
        // wall.DORotate(new Vector3(90, 0, 0), 2);
        wall.DOScale(new Vector3(0, 0, 0), 2);
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
        yield return new WaitForSeconds(1.5f);
        inputUI.ShowAlert(AlertType.Wrong, false);
    }
}
