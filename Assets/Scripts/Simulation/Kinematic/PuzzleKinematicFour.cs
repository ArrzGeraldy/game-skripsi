using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PuzzleKinematicFour : MonoBehaviour, PuzzleHandlerI
{
    public float targetAnswer;
    public ParabolaUI inputUI;
    public Transform cube;
    public GameObject wallSide;
    public Transform interact;

    void Start()
    {
        cube.DORotate(new Vector3(0, 360f, 0), 2.66f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }

    void Update()
    {
    }

    public void CheckAnswer()
    {
        cube.DOScale(1, 1.2f);

        if (Mathf.Approximately(inputUI.v0Slider.value, targetAnswer))
        {
            StartCoroutine(CorrectAnswer());
            GameLevelManager.Instance.IncreasePuzzle();


            Debug.Log("BENARR !!; DEGAN VELO: " + inputUI.v0Slider.value);
            
        }
        else
        {
            float selisih = inputUI.v0Slider.value - targetAnswer;
            string msg = FuzzyHint.GetHint(selisih,20f, "V0");
            inputUI.wrongLabel.text = msg;
            StartCoroutine(WrongAnswer());
            Player.Instance.LoseLife();

        }
    }

    public IEnumerator CorrectAnswer()
    {
        interact.DOLocalMoveY(-5f,1);

        inputUI.ShowAlert(AlertType.Success, true);
        
        yield return new WaitForSeconds(1.5f);
        inputUI.Hide();
        Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);

        yield return new WaitForSeconds(1.5f);
        wallSide.SetActive(true);



        yield return null;
    }

    public void OnPuzzleActivated()
    {
        wallSide.SetActive(false);
        inputUI.OnStartSimulation += OnStartSimulation;
        inputUI.OnTimerReachedTarget += CheckAnswer;
    }

    public void OnPuzzleDeactivated()
    {
        wallSide.SetActive(true);
        inputUI.OnStartSimulation -= OnStartSimulation;
        inputUI.OnTimerReachedTarget += CheckAnswer;

    }

    public void OnStartSimulation()
    {
        cube.localScale = new Vector3(1,1,1);
        cube.DOScale(Vector3.zero, 1.1f).SetEase(Ease.OutCubic);
     
    }

    public IEnumerator WrongAnswer()
    {
        
        inputUI.ShowAlert(AlertType.Wrong, true);
        
        yield return new WaitForSeconds(2f);
        inputUI.ShowAlert(AlertType.Wrong, false);
        
        yield return new WaitForSeconds(1.5f);

    }

}
