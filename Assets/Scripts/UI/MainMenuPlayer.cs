using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }



    public void OnResume()
    {
        Player.Instance.lockInput = false;

        Player.Instance.UnregisterUI();
        Time.timeScale = 1f; 
        gameObject.SetActive(false);
    }

    public void ExitLevel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("LevelMenu");

        // Application.Quit();
        // #if UNITY_EDITOR
        //     UnityEditor.EditorApplication.isPlaying = false;
        // #endif
    }
}
