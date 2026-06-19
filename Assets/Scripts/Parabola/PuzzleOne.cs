using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPuzzleHandler
{
    void OnPuzzleActivated();
    void OnPuzzleDeactivated();
}

public class PuzzleOne : MonoBehaviour, IPuzzleHandler
{
    public Canvas InputParabola;
    [SerializeField]ParabolaUI parabolaUI;
    public float targetLength;
    [SerializeField] float targetV0;
    public Canon canon;
    public Transform gravityObstacle;
    public Transform cubes;
    public BoxCollider colliderObstacle;

    void Start()
    {
        parabolaUI = InputParabola.GetComponent<ParabolaUI>();
        if(parabolaUI == null)
        {
            Debug.LogError("PARABOLA UI NULL");
            return;
        }

        foreach(Transform cube in cubes)
        {
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            if(rb)
                rb.useGravity = false;
        }

        // targetV0 = targetLength/parabolaUI.tTarget;
        // parabolaUI.OnTimerReachedTarget += CheckPlayerAnswer;
        
    }

    void Update()
    {
        if(parabolaUI == null)
        {
            Debug.LogError("PARABOLA UI NULL");
            return;
        }

    }

    void CheckPlayerAnswer()
    {
        // Tips: Untuk float, sebaiknya gunakan toleransi error kecil (Mathf.Approximately)
        // daripada '==' murni, demi menghindari isu presisi micro-float pada slider.
        if (Mathf.Approximately(parabolaUI.v0Slider.value, targetV0))
        {
            Debug.Log("BENARR !!; DEGAN VELO: " + parabolaUI.v0Slider.value);
            StartCoroutine(CorrectAnswer());
            
        }
        else
        {
            Debug.Log("SALAHHH !!; DEGAN VELO: " + parabolaUI.v0Slider.value);
            string hint = parabolaUI.v0Slider.value > targetV0 ? "besar" : "kecil";
            string msg = "v<sub>0</sub> terlalu " + hint;
            parabolaUI.wrongLabel.text = msg;
            StartCoroutine(WrongAnswer());

        }

        canon.DestroyBullet();
    }

    IEnumerator CorrectAnswer()
    {
        parabolaUI.ShowAlert(AlertType.Success, true);
        
        Vector3 initialScale = gravityObstacle.localScale;
        Vector3 targetScale = new Vector3(0.1f, 0.1f, 0.1f); // Ubah ke target ukuran kecil yang kamu mau
        float duration = 1.0f;
        float timeElapsed = 0f;

        while (timeElapsed < 1.0f)
        {
            timeElapsed += Time.deltaTime / duration;
            
            // Mengubah scale secara halus dari awal ke target berdasarkan waktu
            gravityObstacle.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed);
            
            yield return null; // Tunggu ke frame berikutnya
        }

        foreach(Transform cube in cubes)
        {
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            if(rb)
                rb.useGravity = true;
        }

        colliderObstacle.enabled = false;

        yield return new WaitForSeconds(1.5f);
        Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);
        parabolaUI.Hide();
        yield return null;
    }

    IEnumerator WrongAnswer()
    {
        parabolaUI.ShowAlert(AlertType.Wrong, true);

        yield return new WaitForSeconds(2f);
        parabolaUI.ShowAlert(AlertType.Wrong, false);

        yield return null;
    }

    // Jangan lupa lepaskan registrasi event saat objek dihancurkan agar tidak memory leak
    void OnDestroy()
    {
        if (parabolaUI != null)
        {
            parabolaUI.OnTimerReachedTarget -= CheckPlayerAnswer;
        }
    }

    public void OnPuzzleActivated()
    {
        parabolaUI.OnTimerReachedTarget += CheckPlayerAnswer;
    }

    public void OnPuzzleDeactivated()
    {
        parabolaUI.OnTimerReachedTarget -= CheckPlayerAnswer;
    }

    
}
