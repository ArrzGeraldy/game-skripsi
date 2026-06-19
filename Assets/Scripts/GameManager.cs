using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioSource audioSource;
    public static GameManager Instance;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        
    }

    public void PlayeOneShot(AudioClip clip)
    {
        audioSource.pitch = Random.Range(0.8f, 1.1f);
        audioSource.volume = 1;
        audioSource.PlayOneShot(clip);
    }
}
