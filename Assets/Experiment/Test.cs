using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        DB.Init();
    }
    void Start()
    {
        var levels = Level.GetAll();

        foreach (var level in levels)
        {
            Debug.Log($"ID: {level.id}, Name: {level.name}, Scene: {level.scene_name}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
