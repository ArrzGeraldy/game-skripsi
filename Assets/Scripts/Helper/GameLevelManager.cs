using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance;
    public Vector3 checkPoint;

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
        checkPoint = Player.Instance.transform.position;
        Debug.Log("Check Point: " + checkPoint);
        UpdateUIInfo();
    }

    public void OnCheckPoint()
    {
        Debug.Log("CheckPoint !");
        checkPoint = Player.Instance.transform.position;
    }

    public void OnPlayerFall()
    {
        Debug.Log("Fall !");
        // Player.Instance.cc.Move(checkPoint)
        StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        Player.Instance.cc.enabled = false;
        yield return null; // ← tunggu 1 frame dulu setelah disable!
        
        yield return new WaitForSeconds(1f);
        Debug.Log("player when fall: " +Player.Instance.transform.position);
        Debug.Log("Check Point: " + checkPoint);
        
        Player.Instance.transform.position = checkPoint;
        yield return null;
        
        Player.Instance.cc.enabled = true;
    }

    public void IncreasePuzzle()
    {
        currentPuzzle++;
        if(currentPuzzle >= maxPuzzle)
        {
            StartCoroutine(HandleWin());
            Debug.Log("WINN !");
        }
        UpdateUIInfo();
    }

    IEnumerator HandleWin()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Win");
    }

    public void UpdateUIInfo()
    {
        labelLife.text = $"x{Player.Instance.currentLife}";
        labelPuzzle.text = $"{currentPuzzle}/{maxPuzzle}";
    }
}
