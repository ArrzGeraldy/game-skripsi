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

    void Start()
    {
        cube.DORotate(new Vector3(0, 360, 0), 0.5f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void CheckAnswer()
    {
        if (Mathf.Approximately(inputUI.v0Slider.value, targetAnswer))
        {
            StartCoroutine(CorrectAnswer());

            Debug.Log("BENARR !!; DEGAN VELO: " + inputUI.v0Slider.value);
            
        }
        else
        {
            Debug.Log("SALAHHH !!; DEGAN VELO: " + inputUI.v0Slider.value);
            string hint = inputUI.v0Slider.value > targetAnswer ? "besar" : "kecil";
            string msg = "v<sub>0</sub> terlalu " + hint;
            inputUI.wrongLabel.text = msg;
            StartCoroutine(WrongAnswer());
        }
    }

    public IEnumerator CorrectAnswer()
    {
        wallSide.SetActive(true);
        throw new System.NotImplementedException();
    }

    public void OnPuzzleActivated()
    {
        wallSide.SetActive(false);
        inputUI.OnStartSimulation += OnStartSimulation;
    }

    public void OnPuzzleDeactivated()
    {
        wallSide.SetActive(true);

    }

    public void OnStartSimulation()
    {
        cube.localScale = new Vector3(1,1,1);
        cube.DOScale(0, 1.2f);
        cube.DORotate(new Vector3(0, 90, 0), 1.2f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public IEnumerator WrongAnswer()
    {
        throw new System.NotImplementedException();
    }

}
