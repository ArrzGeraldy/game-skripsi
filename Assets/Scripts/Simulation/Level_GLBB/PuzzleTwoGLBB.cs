using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PuzzleTwoGLBB : MonoBehaviour, PuzzleHandlerI
{
    public float targetAnswerv0;
    public ParabolaUI inputUI;
    public Transform door;
    public Ease ease;
    public Transform obs;
    Vector3 initObsPos;

    public void CheckAnswer()
    {


        inputUI._currentConfig.canon.SetBulletGravity(true);
        if(Mathf.Approximately(targetAnswerv0, inputUI.v0Slider.value) )
        {
            StartCoroutine(CorrectAnswer());
            GameLevelManager.Instance.IncreasePuzzle();

        }
        else
        {

            float deltaV0 = inputUI.v0Slider.value - targetAnswerv0;
            string msg = FuzzyHint.GetHint(deltaV0,20f, "v<sub>0</sub>");
            inputUI.wrongLabel.text = msg;
            StartCoroutine(WrongAnswer());
            Player.Instance.LoseLife();

        }
    }

    public IEnumerator CorrectAnswer()
    {
        door.DOLocalMoveY(54.6f, 2f).SetEase(ease);

        inputUI.ShowAlert(AlertType.Success, true);
        yield return new WaitForSeconds(4f);

        Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);
        inputUI.Hide();
        
    }

    public IEnumerator WrongAnswer()
    {
        inputUI.ShowAlert(AlertType.Wrong, true);
        yield return new WaitForSeconds(4.5f);
        inputUI.ShowAlert(AlertType.Wrong, false);
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
        obs.localPosition = initObsPos;

    }

    void Start()
    {
        initObsPos = obs.localPosition;    
    }
}
