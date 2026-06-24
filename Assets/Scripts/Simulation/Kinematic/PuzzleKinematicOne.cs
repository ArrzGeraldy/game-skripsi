using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PuzzleKinematicOne : MonoBehaviour, PuzzleHandlerI
{
    public float targetAnswer;
    public ParabolaUI inputUI;
    public Transform cubes;
    public Transform gravityObstacle;
    public BoxCollider wall;

    void Start()
    {
        
        foreach(Transform cube in cubes)
        {
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            if(rb)
                rb.useGravity = false;
        }
    }

   

    public void CheckAnswer()
    {
        if (Mathf.Approximately(inputUI.v0Slider.value, targetAnswer))
        {
            StartCoroutine(CorrectAnswer());
            Debug.Log("BENARR !!; DEGAN VELO: " + inputUI.v0Slider.value);
            GameLevelManager.Instance.IncreasePuzzle();
            
        }
        else
        {
            float selisih = inputUI.v0Slider.value - targetAnswer;
            string msg = FuzzyHint.GetHint(selisih,20f, "V0");
            inputUI.wrongLabel.text = msg;
            StartCoroutine(WrongAnswer());
            Player.Instance.LoseLife();
        }

        GameLevelManager.Instance.UpdateUIInfo();
        inputUI._currentConfig.canon?.DestroyBullet();
    }

    public IEnumerator CorrectAnswer()
    {
        inputUI.ShowAlert(AlertType.Success, true);
        
        gravityObstacle.DOScale(0, 1);
        yield return new WaitForSeconds(2.0f);

        foreach(Transform cube in cubes)
        {
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            if(rb)
                rb.useGravity = true;
        }

        wall.enabled = false;

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
        Debug.Log("puzzle activeted: " + targetAnswer);
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
    }

 
}
