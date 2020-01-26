using System;
using UnityEngine;
using Vuforia;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    protected GameObject placementIndicatorPrefab;
    [Header("Object")]
    [SerializeField]
    protected GameObject objectToPlace;
    [SerializeField]
    protected bool clone = true;
    [SerializeField]
    protected float placementHeight = 0f;
    [SerializeField]
    protected bool faceCamera = true;

    public GameObject PlacementIndicatorPrefab
    {
        get => placementIndicatorPrefab;
        set {
            if (value == null)
                throw new UnityException("The 'PlacementIndicatorPrefab' field cannot be equal to 'null'");

            placementIndicatorPrefab = value;
        }
    }
    public GameObject ObjectToPlace
    {
        get => objectToPlace;
        set
        {
            if (value == null)
                throw new UnityException("The 'ObjectToPlace' field cannot be equal to 'null'");

            objectToPlace = value;
        }
    }
    public bool Clone { get => clone; set => clone = value; }
    public float PlacementHeight { get => placementHeight; set => placementHeight = value; }
    public bool FaceCamera
    {
        get => faceCamera;
        set {
            if (value && (mainCamera = Camera.main) == null)
                throw new UnityException("Couldn't find a main camera in the current scene");
            
            faceCamera = value;
        }
    }

    protected SmartTerrain smartTerrain;
    protected Transform placementIndicator;
    protected Camera mainCamera;

    protected void Awake()
    {
        SmartTerrainARController.Instance.AutoInitAndStartTracker = true;
        VuforiaBehaviour.Instance.StartEvent += StartAfterVuforia;
    }

    protected void StartAfterVuforia()
    {
        if ((smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>()) == null)
        {
            Debug.LogError("ObjectPlacer: Coudn't get an instance of Vuforia's SmartTerrain. Disabling the script");
            enabled = false;
            return;
        }
    }

    protected void OnEnable()
    {
        if (objectToPlace == null)
        {
            Debug.LogError("ObjectPlacer: The 'ObjectToPlace' field cannot be left unassigned. Disabling the script");
            enabled = false;
            return;
        }

        if (faceCamera && (mainCamera = Camera.main) == null)
        {
            Debug.LogError("ObjectPlacer: Couldn't find a main camera in the current scene. Disabling the script");
            enabled = false;
            return;
        }

        if (placementIndicatorPrefab == null)
        {
            Debug.LogError("ObjectPlacer: The 'PlacementIndicatorPrefab' field cannot be left unassigned. Disabling the script");
            enabled = false;
            return;
        }
        var obj = Instantiate(placementIndicatorPrefab);
        obj.SetActive(false);
        placementIndicator = obj.transform;
    }

    protected void Update()
    {
        if (smartTerrain == null) return;
        
        if(smartTerrain.HitTest(new Vector2(0.5f, 0.5f), 1.4f, out HitTestResult result))
        {
            placementIndicator.position = result.Position;
            placementIndicator.rotation = faceCamera ? GetFaceCameraRotation(placementIndicator.position) : result.Rotation;
            
            placementIndicator.gameObject.SetActive(true);
        }
        else
            placementIndicator.gameObject.SetActive(false);
    }

    protected void OnDisable()
    {
        if (placementIndicator != null)
        {
            Destroy(placementIndicator.gameObject);
            placementIndicator = null;
        }
    }

    protected void OnDestroy()
    {
        VuforiaBehaviour.Instance.StartEvent -= StartAfterVuforia;
    }

    public void Place()
    {
        if (!enabled || smartTerrain == null) return;

        if (!smartTerrain.HitTest(new Vector2(0.5f, 0.5f), 1.4f, out HitTestResult result))
            return;

        /*var anchor = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>().CreatePlaneAnchor("Anchor_" + Guid.NewGuid().ToString(), result);
        if (anchor == null)
            throw new UnityException("Vuforia was unable to create a plane anchor");*/
        
        GameObject obj = clone ? Instantiate(objectToPlace) : objectToPlace;
        obj.transform.position = result.Position + new Vector3(0f, placementHeight, 0f);
        obj.transform.rotation = faceCamera ? GetFaceCameraRotation(obj.transform.position) : result.Rotation;
        obj.SetActive(true);
    }

    private Quaternion GetFaceCameraRotation(Vector3 objPosition)
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        cameraPosition.y = objPosition.y;
        return Quaternion.LookRotation(objPosition - cameraPosition);
    }
}
