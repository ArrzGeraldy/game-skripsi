using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelMenuController : MonoBehaviour
{
    List<Level> levels = new List<Level>();
    public GameObject btnLevelPrefab;
    public Transform containerLevel;

    void Awake()
    {
        DB.Init();
    }

    void Start()
    {
        levels = Level.GetAll();
        foreach(var l in levels)
        {
            var levelProgress = LevelProgress.GetByLevel(l.id);
            int high_score = 0;
            if(levelProgress != null)
                high_score = levelProgress.high_score;

            GameObject btn = Instantiate(btnLevelPrefab, containerLevel);
            btn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = l.name + " || Score: " + high_score.ToString();

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                LoadLevel(l);
            });

            Debug.Log($"Created button: {l.name}");
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }

    public void LoadLevel(Level level)
    {
        PlayerPrefs.SetInt("current_level_id", level.id);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f; 
        SceneManager.LoadScene(level.scene_name);
    }

    public void Exit()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }


}
