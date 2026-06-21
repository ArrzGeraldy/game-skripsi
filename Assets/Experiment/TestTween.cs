using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestTween : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // transform.DOMoveX(2, 2);     
        // transform.DORotate(new Vector3(0, 180,0), 2);
        transform.DOScaleX(4, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
