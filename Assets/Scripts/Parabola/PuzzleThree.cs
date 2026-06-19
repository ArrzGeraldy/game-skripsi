using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleThree : MonoBehaviour
{
    bool isHit;

    void OnCollisionEnter(Collision col)
    {
        if (isHit) return;

        if (col.transform.CompareTag("Bullet"))
        {
            isHit = true;
            StartCoroutine(LoadWin());
        }
    }

    IEnumerator LoadWin()
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Win");
    }

}
