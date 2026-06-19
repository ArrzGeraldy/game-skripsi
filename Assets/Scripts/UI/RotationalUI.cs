using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RotationalUI : MonoBehaviour
{
    public Slider kecepatanSudutSlider;
    public TextMeshProUGUI label;

    public RectTransform orbitContainer;
    public RectTransform ballGreen;
    Vector3 initBallGreenPos;

    public float angle = 0;
    public float w = 0;
    public float wTemp = 0;

    public TextMeshProUGUI labelTotalPutaran;
    public float totalPutaran = 0;
    public float needRotation = 3.0f;
    public float tolerance = 0.05f;


    bool startSimulation = false;

    // timer
    public float simulationCountDown = 3f;
    public float simulationCountDownTimer = 0f;

    public float simulationDuration = 6f;
    public float simulationTimer = 0f;


    public Image countDownPanel;
    public TextMeshProUGUI countDownLabel;
    public TextMeshProUGUI timerSimulationLabel;

    public Transform rocketPad;
    public float rocketTranslateTargetY = 0.5f;
    public float rocketAnimationDuration = 2f;

    void Start()
    {
        countDownPanel.gameObject.SetActive(false);
        initBallGreenPos = ballGreen.position;
        timerSimulationLabel.text = $"Timer: {simulationDuration:F0}s";
        UpdateUI();
    }

    void Update()
    {
     
        angle += w * Time.deltaTime;

        if (startSimulation)
            totalPutaran = angle / (2 * Mathf.PI);

        orbitContainer.localEulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
        labelTotalPutaran.text = $"Total Putaran: {totalPutaran:F1}";
        ballGreen.position = initBallGreenPos;
    }

    public void OnSliderChanger()
    {
        float raw = kecepatanSudutSlider.value;
        w = raw * Mathf.PI / 6;
        wTemp = w;

        UpdateUI();
    }

    public void OnButtonStart()
    {
        StopAllCoroutines();

        w = 0;
        angle = 0;
        totalPutaran = 0;
        startSimulation = false;

        orbitContainer.localEulerAngles = Vector3.zero;
        labelTotalPutaran.text = $"Total Putaran: {totalPutaran:F1}";

        simulationCountDownTimer = simulationCountDown;
        timerSimulationLabel.text = $"Timer: {simulationDuration:F0}s";

        countDownPanel.gameObject.SetActive(true);


        StartCoroutine(UpdateSimulationCountDown());
    }

    IEnumerator UpdateSimulationCountDown()
    {
        while (simulationCountDownTimer > 0f)
        {
            simulationCountDownTimer -= Time.deltaTime;
            countDownLabel.text = $"{simulationCountDownTimer:F0}";
            yield return null;
        }

        countDownLabel.text = $"{0}";
        countDownPanel.gameObject.SetActive(false);
        startSimulation = true;
        w = wTemp;
        simulationTimer = simulationDuration;

        StartCoroutine(UpdateSimulationTimer());
    }

    IEnumerator UpdateSimulationTimer()
    {
        while (simulationTimer > 0f)
        {
            simulationTimer -= Time.deltaTime;
            timerSimulationLabel.text = $"Timer: {simulationTimer:F0}s";

            yield return null;
        }

        angle += w * simulationTimer;
        timerSimulationLabel.text = $"Timer: {0}s";

        w = 0f;
        startSimulation = false;

        if(Mathf.Abs(totalPutaran - needRotation) <= tolerance)
        {
            Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);
            Debug.Log("BENAR !!");
            StartCoroutine(RocketAnimation());
        }
        else
            Debug.Log("SALAH !!");

    }

    IEnumerator RocketAnimation()
    {
        float speed = 1f;
        float duration = 2f;
        float elapsed = 0f;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;

            rocketPad.position += new Vector3(0, Time.deltaTime, 0);

            yield return null;
        }
    }

    void UpdateUI()
    {
        label.text = "Kecepatan Sudut (ω): " + $"{w:F2} rad/s" + $" || deg: {(w * Mathf.Rad2Deg):F2}";
    }
}