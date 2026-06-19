using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public Button btnOpen;

    public void SetOpen(GameObject obj)
    {

 
        Time.timeScale = 1;
        obj.SetActive(true);

        if (!Player.Instance)
        {
            Debug.LogError("Player Instance null !!!");
            return;
        }

        Player.Instance.RegisterUI();

        Player.Instance.lockInput = true;
        Player.Instance.anim.SetFloat("speed",0);

    }
    public void SetClose(GameObject obj)
    {
        if (!Player.Instance)
        {
            Debug.LogError("Player Instance null !!!");
            return;
        }

        Player.Instance.UnregisterUI();
        Time.timeScale = 1;
        obj.SetActive(false);

        Player.Instance.lockInput = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {

            btnOpen.onClick.Invoke();
        }
    }
}
