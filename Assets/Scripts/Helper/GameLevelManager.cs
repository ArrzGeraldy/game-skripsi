using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance;
    public Transform checkPoint;

    public int maxPuzzle = 7;
    public int currentPuzzle = 0;

    [Header("Refrences")]
    public TextMeshProUGUI labelLife;
    public TextMeshProUGUI labelPuzzle;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUIInfo();
    }

    public void OnCheckPoint()
    {
        Debug.Log("CheckPoint !");
        checkPoint = Player.Instance.transform;
    }

    public void OnPlayerFall()
    {
        Debug.Log("Fall !");
        StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Start Coroutine !");
        Debug.Log(checkPoint.position);

        Player.Instance.cc.enabled = false;
        yield return null;
        Debug.Log("cc: " + Player.Instance.cc.enabled);

        Player.Instance.transform.position = checkPoint.position;
        yield return null;
        Player.Instance.cc.enabled = true;
    }

    public void IncreasePuzzle()
    {
        currentPuzzle++;
        if(currentPuzzle >= maxPuzzle)
        {
            Debug.Log("WINN !");
        }
    }

    public void UpdateUIInfo()
    {
        labelLife.text = $"x{Player.Instance.currentLife}";
        labelPuzzle.text = $"{currentPuzzle}/{maxPuzzle}";
    }
}
