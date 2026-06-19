using UnityEngine;

public class SimulationTriggerZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas inputCanvas;
    [SerializeField] private Transform interactPoint;
     [SerializeField] private MonoBehaviour puzzleHandler;

    [Header("Camera")]
    [SerializeField] private Vector3 sideViewEuler = new Vector3(15, -90, 0);

    [Header("UI Config")]
    [SerializeField] private SimulationUIConfig uiConfig;

    private ISimulationUI _simulationUI;
    private bool _hasCollided;
    private bool _hasError;
    private IPuzzleHandler _puzzle; 

    void Start()
    {
        if (!ValidateReferences()) return;

        _simulationUI = inputCanvas.GetComponent<ISimulationUI>();

        if (_simulationUI == null)
        {
            _hasError = true;
            Debug.LogError("ISimulationUI not found on inputCanvas!");
            return;
        }

        if(puzzleHandler)
            _puzzle = puzzleHandler as IPuzzleHandler;



        _simulationUI.Hide();
    }

    void OnTriggerEnter(Collider col)
    {
        if (_hasError || _hasCollided) return;
        if (!col.CompareTag("Player")) return;

        _puzzle?.OnPuzzleActivated();

        SnapPlayerToInteractPoint();
        Player.Instance.SwitchVCam(VCamType.VCAM_SIDEVIEW);
        Player.Instance.SetSideViewRotation(sideViewEuler);
        Player.Instance.SetSideViewRotation(sideViewEuler);
        Debug.Log(uiConfig);
        _simulationUI.Show(uiConfig);
        _hasCollided = true;
    }

    void OnTriggerExit(Collider col)
    {
        if (_hasError || !_hasCollided) return;
        if (!col.CompareTag("Player")) return;
        _puzzle?.OnPuzzleDeactivated();


        _hasCollided = false;
    }

    private void SnapPlayerToInteractPoint()
    {
        var player = Player.Instance;
        player.cc.enabled = false;
        player.transform.position = new Vector3(
            interactPoint.position.x,
            player.transform.position.y,
            interactPoint.position.z
        );
        player.cc.enabled = true;
    }

    private bool ValidateReferences()
    {
        if (!Player.Instance)
        {
            _hasError = true;
            Debug.LogError("Player Instance is null!");
            return false;
        }

        if (!inputCanvas)
        {
            _hasError = true;
            Debug.LogError("inputCanvas is null!");
            return false;
        }

        return true;
    }
}