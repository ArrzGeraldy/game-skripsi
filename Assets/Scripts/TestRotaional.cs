using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotaional : MonoBehaviour
{
    // Start is called before the first frame update

    public float periode;
    public float minimumT;
    public float r = 1f;

    [Header("akan di hitung")]
    public float T;
    public float w;
    public float wDeg;

    void Start()
    {
        T = minimumT / periode;
        w = 2 * Mathf.PI / T * r;

        wDeg = w * 180f/Mathf.PI;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, wDeg * Time.deltaTime);
    }
}
