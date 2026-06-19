using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SlashVFX : MonoBehaviour
{
    public GameObject slash;

    void Awake()
    {
        Debug.Log("awake slash vfx");
    }

    public void DrawForward(float type = 1,float rotate = 0f)
    {
        Quaternion rot = transform.rotation  * Quaternion.Euler(0, 0, 45f * rotate);
        GameObject currSlash = Instantiate(slash, transform.position, rot);
        SlashRibbon _slash = currSlash.GetComponent<SlashRibbon>();
        if (!_slash)
        {
            Debug.LogError("Slash ribbon null in Draw Forward");
            return;
        }
        _slash.DrawSlashCubic(type);
    }
}
