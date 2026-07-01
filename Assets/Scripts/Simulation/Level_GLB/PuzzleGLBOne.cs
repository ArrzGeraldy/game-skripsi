using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGLBOne : MonoBehaviour, PuzzleHandlerI
{
    public float targetAnswerv0;
    public ParabolaUI inputUI;
    public Rigidbody rb;

    public void CheckAnswer()
    {
        inputUI._currentConfig.canon.SetBulletGravity(true);
        if(Mathf.Approximately(targetAnswerv0, inputUI.v0Slider.value) )
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);

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

        inputUI.ShowAlert(AlertType.Success, true);
        yield return new WaitForSeconds(2.5f);
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
        if(Mathf.Approximately(targetAnswerv0, inputUI.v0Slider.value))
        {
            rb.isKinematic = false;

        }
        else
        {
            rb.isKinematic = true;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        // rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
