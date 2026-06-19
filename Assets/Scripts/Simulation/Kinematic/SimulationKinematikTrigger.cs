using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationKinematikTrigger : MonoBehaviour
{
    public bool _hasCollided;
    public Transform centerPoint;
    private string errMsg;

    [SerializeField] private ParabolaUI inputUI;

    [Header("Camera")]
    [SerializeField] private Vector3 sideViewEuler = new Vector3(15, -90, 0);
    [SerializeField] private Vector3 sideViewPos = new Vector3(14, 4, 8.5f);

    [Header("UI Config")]
    [SerializeField] private SimulationUIConfig uiConfig;

    [Header("Puzzle")]
    [SerializeField] public MonoBehaviour puzzle;
    [SerializeField] public PuzzleHandlerI puzzleHandler;


    void Start()
    {
        checkError();

        puzzleHandler = puzzle as PuzzleHandlerI;
    }

    void OnTriggerEnter(Collider col)
    {
        if (_hasCollided) return;
        if (!col.CompareTag("Player")) return;

        SnapPlayerToCenter();
        Player.Instance.SwitchVCam(VCamType.VCAM_SIDEVIEW);
        Player.Instance.SetSideViewPosition(sideViewPos);

        inputUI.Show(uiConfig);
        puzzleHandler.OnPuzzleActivated();
        _hasCollided = true;
    }

    void OnTriggerExit(Collider col)
    {
        if (!_hasCollided) return;
        if (!col.CompareTag("Player")) return;

        _hasCollided = false;
    }

    private void SnapPlayerToCenter()
    {
        var player = Player.Instance;
        player.cc.enabled = false;
        player.transform.position = new Vector3(
            centerPoint.position.x,
            player.transform.position.y,
            centerPoint.position.z
        );
        player.cc.enabled = true;
    }


    void checkError()
    {
        if(!inputUI)
        {
            errMsg = "CANVAS INPUT NULL";
        }
        if(!centerPoint) errMsg = "CENTER POINT NULL";
    }
}
