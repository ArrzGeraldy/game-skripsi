using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalculateDIstance : MonoBehaviour
{
    public Transform pos1;
    public Transform pos2;

    // Start is called before the first frame update
    void Start()
    {

        if(pos1 && pos2)
        {
            TextMeshPro text = GetComponent<TextMeshPro>();
            float distance = pos2.position.z - pos1.position.z;
            text.text = $"{distance} meter";
        }

    }

    // Update is called once per frame
    void Update()
    {
           if(pos1 && pos2)
        {
            TextMeshPro text = GetComponent<TextMeshPro>();
            float distance = pos2.position.z - pos1.position.z;
            text.text = $"{distance} meter";
        }
    }
}
