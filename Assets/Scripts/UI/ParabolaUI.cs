using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[System.Serializable]
public class SimulationUIConfig
{
    public Canon canon;
    public bool useVelo;
    public bool useAngle;
    public bool useTimer;
    public float tTarget;
    public string questionText;

}

public interface ISimulationUI
{
    void Show(SimulationUIConfig config);
    void Hide();
}


public class ParabolaUI : MonoBehaviour, ISimulationUI
{
    [Header("References")]
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
    public TextMeshProUGUI tLabel;
    private float t = 0;
    public float CurrTime => t;

    [Header("Cosinus")]
    public TextMeshProUGUI cosLabel;
    public TextMeshProUGUI sinLabel;

    [Header("Question")]

    public bool startTimer = false;
    public Action OnTimerReachedTarget;
    public Action OnStartSimulation;
    public bool inSimulation = false;

    private SimulationUIConfig _currentConfig;

    void Awake()
    {

        successAlert.SetActive(false);
        wrongAlert.SetActive(false);
    }

    // ISimulationUI
    public void Show(SimulationUIConfig config)
    {
        if (config == null)
        {
            Debug.LogError("SimulationUIConfig is null!");
            return;
        }

        if (!config.canon)
        {
            Debug.LogError("Canon is null in SimulationUIConfig!");
            return;
        }


        Player.Instance.RegisterUI();
    
        _currentConfig = config;
        gameObject.SetActive(true);
        successAlert.SetActive(false);
        wrongAlert.SetActive(false);

        timerPanel.SetActive(config.useTimer);
        angelPanel.SetActive(config.useAngle);
        veloPanel.SetActive(config.useVelo);
        cosinusPanel.SetActive(config.useAngle);
        questionLabel.gameObject.SetActive(!string.IsNullOrEmpty(_currentConfig.questionText));


        if (config.useAngle)
        {
            cosLabel.text = $"cos({0}): {Mathf.Cos(0 * Mathf.Deg2Rad):F3}";
            sinLabel.text = $"sin({0}): {Mathf.Sin(0 * Mathf.Deg2Rad):F3}";
        }

        t = 0;
        UpdateLabel();
    }


    public void Hide()
    {
        if(_currentConfig != null && _currentConfig.canon)
        {
            _currentConfig.canon.Reset();
            angleSlider.value = _currentConfig.canon.angle;
            v0Slider.value = _currentConfig.canon.v0;
            t = 0;
        }

        Player.Instance.UnregisterUI();


        gameObject.SetActive(false);
    }

    public void OnAngleSliderChange()
    {
        float raw = angleSlider.value;
        float snapped = Mathf.Round(raw / 5f) * 5f;
        angleSlider.SetValueWithoutNotify(snapped);

        if (_currentConfig.useAngle)
        {
            cosLabel.text = $"cos({snapped}): {Mathf.Cos(snapped * Mathf.Deg2Rad):F3}";
            sinLabel.text = $"sin({snapped}): {Mathf.Sin(snapped * Mathf.Deg2Rad):F3}";
        }

        _currentConfig.canon.SetAngle(snapped);
        UpdateLabel();
    }

    public void OnV0Change()
    {
        _currentConfig.canon.v0 = v0Slider.value;
        UpdateLabel();
    }

    public void TriggerShoot()
    {
        if(inSimulation) return;
        t = 0;
        inSimulation = true;

        HideAllAlerts();
        OnStartSimulation?.Invoke();
        startTimer = true;
        tLabel.text = $"t: {t:F2}s";
        _currentConfig.canon.Shoot();
    }

    public void Update()
    {
        if (_currentConfig == null || !_currentConfig.useTimer || !startTimer) return;

        t += Time.deltaTime;
        float displayedTime = Mathf.Min(t, _currentConfig.tTarget);
        tLabel.text = $"t = {displayedTime:F2}s";

        if (t >= _currentConfig.tTarget)
        {
            t = _currentConfig.tTarget;
            startTimer = false;
            inSimulation = false;
            OnTimerReachedTarget?.Invoke();

        }
    }

    public void ShowAlert(AlertType type, bool isActive)
    {
        if (isActive) HideAllAlerts();

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
        Hide();
    }

    private void HideAllAlerts()
    {
        successAlert.SetActive(false);
        wrongAlert.SetActive(false);
    }

    private void UpdateLabel()
    {
        if (_currentConfig.useAngle)
            angleText.text = "Sudut (θ): " + _currentConfig.canon.angle.ToString() + "°";

        if (_currentConfig.useVelo)
            v0Text.text = "Kecepatan Awal (v<sub>0</sub>): " + _currentConfig.canon.v0.ToString() + " m/s";

        if (_currentConfig.useTimer)
            tLabel.text = $"t: {t:F2}s";

        if (!string.IsNullOrEmpty(_currentConfig.questionText))
            questionLabel.text = _currentConfig.questionText;
    }
}