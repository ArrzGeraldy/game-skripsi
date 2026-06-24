using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PuzzleKinematicFive : MonoBehaviour, PuzzleHandlerI
{
    public float targetAnswerv0;
    public float targetAnswerAngle;
    public ParabolaUI inputUI;
    public Transform bridge;

    public void CheckAnswer()
    {
        if(Mathf.Approximately(targetAnswerv0, inputUI.v0Slider.value))
        {
            StartCoroutine(CorrectAnswer());
            GameLevelManager.Instance.IncreasePuzzle();

        }
        else
        {
            float deltaV0 = inputUI.v0Slider.value - targetAnswerv0;
            float deltaAngle = inputUI.angleSlider.value - targetAnswerAngle;
            string msg = FuzzyHint.GetHint(deltaV0,20f, "V0");
            string msg2 = FuzzyHint.GetHint(deltaAngle, 60f, "Sudut");
            inputUI.wrongLabel.text = msg + "\n" + msg2;
            StartCoroutine(WrongAnswer());
            Player.Instance.LoseLife();

        }
    }

    public IEnumerator CorrectAnswer()
    {
        bridge.DOScale(new Vector3(9, 2, 9), 3);
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
