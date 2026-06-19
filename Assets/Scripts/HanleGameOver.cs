using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HanleGameOver : MonoBehaviour
{

    bool error;
    void Start()
    {
        if(!Player.Instance)
        {
            Debug.LogError("Player Instance NULL");
            error = true;
        }
    }
    
    void OnTriggerEnter(Collider col)
    {

        if(col.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level_1");
           
        }
    }
}
