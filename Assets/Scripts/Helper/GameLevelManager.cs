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
    public GameObject room8;

    int levelId;

    void Awake()
    {
        levelId = PlayerPrefs.GetInt("current_level_id", -1);
        Debug.Log("LEVEL ID: "+levelId);
        Instance = this;
        DB.Init();
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

    public void ActivateRoom(int val)
    {
        Debug.Log("activate val: " + val);
        if(val == 8)
        {
            room8.SetActive(true);
        }
    }

    public void ExitLevel()
    {
        float numerator = currentPuzzle * Player.Instance.currentLife;
        float denominator = maxPuzzle * Player.Instance.lifes;

        float score = numerator / denominator;

        int finalScore = Mathf.RoundToInt(score * 100f);

        Debug.Log("Score: " + finalScore);

        LevelProgress.UpdateScore(levelId, finalScore);
    }
}
