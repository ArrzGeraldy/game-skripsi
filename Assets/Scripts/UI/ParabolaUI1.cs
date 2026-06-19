using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public enum AlertType
{
    Success, Wrong
}


public class ParabolaUI1 : MonoBehaviour
{
    [Header("References")]
    public Canon canon;
    public GameObject angelPanel;
    public GameObject veloPanel;
    public GameObject timerPanel;
    public TextMeshProUGUI questionLabel;
    public GameObject wrongAlert;
    public GameObject successAlert;
    public GameObject cosinusPanel;
    public TextMeshProUGUI wrongLabel;

    [Header("Angle")]
    public float angleStep = 5f;
    public Slider angleSlider;
    public TextMeshProUGUI angleText;

    [Header("Velocity")]
    public Slider v0Slider;
    public TextMeshProUGUI v0Text;

    [Header("Timer")]
    public bool useTimer = true;
    public float tTarget;
    public TextMeshProUGUI tLabel;
    private float t = 0;
    public float CurrTime => t;

    [Header("Cosinus")]
    public bool useCosinus = false;
    public TextMeshProUGUI cosLabel;
    public TextMeshProUGUI sinLabel;
 

    [Header("Question")]
    public bool useQuest = true;
    public string questionText;

    [Header("Setting")]
    public bool useAngle = true;
    public bool useVelo = true;

    public bool startTimer = false;
    public Action OnTimerReachedTarget;


    void Start()
    {
        Debug.Log("target assign: " + tTarget);
        if(canon == null)
            Debug.LogError("Canon NULL !!");

        timerPanel.SetActive(useTimer);
        angelPanel.SetActive(useAngle);
        veloPanel.SetActive(useVelo);
        cosinusPanel.SetActive(useAngle);
        successAlert.SetActive(false);
        wrongAlert.SetActive(false);

        
        if(useCosinus)
        {
            cosLabel.text = $"cos({0}): {(Mathf.Cos(0 * Mathf.Deg2Rad)):F3}";
            sinLabel.text = $"sin({0}): {(Mathf.Sin(0 * Mathf.Deg2Rad)):F3}";
        }

        questionLabel.gameObject.SetActive(!string.IsNullOrEmpty(questionText));
        UpdateLabel();
    }

    public void OnAngleSliderChange()
    {

        float raw = angleSlider.value;

        float snapped = Mathf.Round(raw/5f) * 5f;

        angleSlider.SetValueWithoutNotify(snapped);

        if(useCosinus)
        {
            cosLabel.text = $"cos({snapped}): {(Mathf.Cos(snapped * Mathf.Deg2Rad)):F3}";
            sinLabel.text = $"sin({snapped}): {(Mathf.Sin(snapped * Mathf.Deg2Rad)):F3}";
        }

        // canon.angle = snapped;
        canon.SetAngle(snapped);
        UpdateLabel();
    }

    public void OnV0Change()
    {
        canon.v0 = v0Slider.value;
        UpdateLabel();
    }

    public void TriggerShoot()
    {
        t = 0;
        HideAllAlerts();
        startTimer = true;
        tLabel.text = $"t: {t:F2}s";
        canon.Shoot();
    }

    public void Update()
    {
        if(useTimer && startTimer)
        {
            t += Time.deltaTime;
            float displayedTime = Mathf.Min(t, tTarget);
            tLabel.text = $"t = {displayedTime:F2}s";

            if(t >= tTarget)
            {
                t = tTarget; 
                startTimer = false;

                // Eksekusi event jika ada script lain yang mendengarkan
                OnTimerReachedTarget?.Invoke();
            }
        }
    }

    public void UpdateUI(bool isUseV, bool isUseA, bool isUseT)
    {
        useTimer = isUseT; useAngle = isUseA; ; useVelo = isUseV;
        timerPanel.SetActive(useTimer);
        angelPanel.SetActive(useAngle);
        veloPanel.SetActive(useVelo);
        cosinusPanel.SetActive(useAngle);
    }

    void UpdateLabel()
    {
        if(useAngle)
            angleText.text = "Sudut (θ): " + canon.angle.ToString() + "°";

        if(useVelo)
            v0Text.text = "Kecepatan Awal (v<sub>0</sub>): " + canon.v0.ToString() + " m/s";

        if(useTimer)
            tLabel.text = $"t: {t:F2}s";


        if(!string.IsNullOrEmpty(questionText))
            questionLabel.text = questionText;
    }

   public void ShowAlert(AlertType type, bool isActive)
    {
        // Nonaktifkan semua terlebih dahulu agar tidak menumpuk (opsional)
        if (isActive) HideAllAlerts();

        // Aktifkan sesuai tipe menggunakan switch case
        switch (type)
        {
            case AlertType.Success:
                successAlert.SetActive(isActive);
                break;
            case AlertType.Wrong:
                wrongAlert.SetActive(isActive);
                break;
         
        }
    }

    public void CloseCanvas()
    {
      
        Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);
        gameObject.SetActive(false);
    }

    private void HideAllAlerts()
    {
        successAlert.SetActive(false);
        wrongAlert.SetActive(false);
    }
}
