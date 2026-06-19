using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleKinematicTwo : MonoBehaviour, PuzzleHandlerI
{
    public float targetAnswer;
    public ParabolaUI inputUI;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void CheckAnswer()
    {
        if (Mathf.Approximately(inputUI.v0Slider.value, targetAnswer))
        {
            Debug.Log("BENARR !!; DEGAN VELO: " + inputUI.v0Slider.value);
            
        }
        else
        {
            Debug.Log("SALAHHH !!; DEGAN VELO: " + inputUI.v0Slider.value);
        }
    }

    public IEnumerator CorrectAnswer()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator WrongAnswer()
    {
        throw new System.NotImplementedException();
    }

    public void OnPuzzleActivated()
    {
        inputUI.OnTimerReachedTarget += CheckAnswer;
    }

    public void OnPuzzleDeactivated()
    {
        inputUI.OnTimerReachedTarget -= CheckAnswer;
    }



    
}
