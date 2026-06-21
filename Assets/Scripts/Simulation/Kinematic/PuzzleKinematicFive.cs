using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PuzzleKinematicFive : MonoBehaviour, PuzzleHandlerI
{
    public float targetAnswer;
    public ParabolaUI inputUI;
    public Transform bridge;

    public void CheckAnswer()
    {
        if(Mathf.Approximately(targetAnswer, inputUI.v0Slider.value))
        {
            StartCoroutine(CorrectAnswer());
        }
        else
        {
            inputUI.wrongLabel.text = "SALAH";
            StartCoroutine(WrongAnswer());
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
        yield return new WaitForSeconds(1.5f);
        inputUI.ShowAlert(AlertType.Wrong, false);
    }

    
}
