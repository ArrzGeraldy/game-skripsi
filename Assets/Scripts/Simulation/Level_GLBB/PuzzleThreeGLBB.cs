using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PuzzleThreeGLBB : MonoBehaviour, PuzzleHandlerI
{
    public float targetAnswerv0;
    public ParabolaUI inputUI;
    public GameObject VolumLight;
    public Light pointLight;
    public Rigidbody rbDoor;
    Renderer volumRenderer;

    public void CheckAnswer()
    {

        volumRenderer.material.SetColor("_Color", Color.green);
        pointLight.color = Color.green;
        
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

        inputUI._currentConfig.canon.SetBulletGravity(false);
        inputUI._currentConfig.canon.BulletForce(100f);

        yield return new WaitForSeconds(2f);
        inputUI.ShowAlert(AlertType.Success, true);
        yield return new WaitForSeconds(2f);

        Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);
        inputUI.Hide();
        
    }

    public IEnumerator WrongAnswer()
    {
        yield return new WaitForSeconds(1.5f);
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
        volumRenderer.material.SetColor("_Color", Color.red);
        pointLight.color = Color.red;
    }

    void Start()
    {
        volumRenderer = VolumLight.GetComponent<Renderer>();
    }
}
