using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShootMotion
{
    Horizontal, Vertical, Parabola
}

public class Canon : MonoBehaviour
{
    public Transform bulletOut;
    public Transform fixedBulletOut;
    public GameObject bullet;

    public static Canon Instance;
    public float angle = 0f;
    public float v0 = 1f;
    GameObject objBullet;
    
    public ShootMotion motion;


    public Transform obsParabola;
    public Transform targetParabola;
    public bool useBulletGravity = true;
    Vector3 dir;

    // init
    public float initAngle;
    float initV0;



    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        switch(motion)
        {
            case ShootMotion.Horizontal:
                dir = -Vector3.forward;
                break;
            case ShootMotion.Vertical:
                dir = Vector3.up;
                break;
            case ShootMotion.Parabola:
                dir = bulletOut.forward;
                break;
        }
    
        initAngle = transform.localEulerAngles.y;
        // angle = 360 - initAngle;
        initV0 = v0;
    }

    public void HandleRotation(float dir, float step)
    {
        float y = transform.localEulerAngles.y;
        if(y > 180) y -= 360;

        y += step * dir;

        float rotY = Mathf.Clamp(y, -90f, -30f);

        transform.localEulerAngles = new Vector3(0, rotY, 0);

    }

    public void SetAngle(float angleValue)
    {
        angle = Mathf.Clamp(angleValue, 0f, 60f);

        float rotY = angle - 90f;

        transform.localEulerAngles = new Vector3(0, rotY, 0);
    }

    public void Shoot()
    {
        Vector3 spawnPosition = (fixedBulletOut != null) ? fixedBulletOut.position : bulletOut.position;
        if(fixedBulletOut != null)
        {
            Debug.Log("FixOut: "+ fixedBulletOut.position );
        }
        Debug.Log("bulletOut: "+ bulletOut.position );
        Debug.Log("spawnPosition: "+ spawnPosition );
        
        objBullet = Instantiate(bullet, spawnPosition, bulletOut.rotation);

        Rigidbody rb = objBullet.GetComponent<Rigidbody>();
    
        rb.useGravity = useBulletGravity;
        if(motion == ShootMotion.Parabola)
            dir = bulletOut.forward;
        rb.velocity = dir * v0;

    }

    public void DestroyBullet()
    {
        if(objBullet != null)
        {
            Destroy(objBullet);
        }
    }

    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.I))
        // {
        //     Debug.Log(targetParabola.position);
        //     Vector3 targetPos = targetParabola ? targetParabola.position :  Vector3.zero;
        //     Debug.Log("Distance to target: " + (targetPos.z -bulletOut.position.z));
        // }
    }


    public void Reset()
    {
        v0 = initV0;
        transform.localEulerAngles = new Vector3(0, initAngle, 0);

    }
}
