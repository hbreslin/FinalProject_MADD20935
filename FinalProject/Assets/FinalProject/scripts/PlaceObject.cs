using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using System;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager), typeof(ARAnchorManager))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField] private GameObject buttonManagerObject;
    private NewButtonManager buttonManager;

    [SerializeField] private string enabledScene;
    [SerializeField] private GameObject selectedPrefab;
    [SerializeField] private float spawnHeight = 0.15f;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private ARAnchorManager anchorManager;
    private bool placementEnabled = false;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool isTouching = false;

    private void Awake()
    {
        buttonManager = buttonManagerObject.GetComponent<NewButtonManager>();
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        anchorManager = GetComponent<ARAnchorManager>();
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += OnFingerDown;
        EnhancedTouch.Touch.onFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= OnFingerDown;
        EnhancedTouch.Touch.onFingerUp -= OnFingerUp;
    }

    public void setPrefab(GameObject GO)
    {
        selectedPrefab = GO;
        Debug.Log("Selected prefab set to: " + GO.name);
    }

    public void enablePlacement(GameObject GO)
    {
        selectedPrefab = GO;
        placementEnabled = true;
    }

    private void OnFingerDown(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0 || IsPointerOverUI(finger.currentTouch.screenPosition))
            return;

        isTouching = true;
    }

    private void OnFingerUp(EnhancedTouch.Finger finger)
    {
        if (finger.index == 0)
            isTouching = false;
    }

    private void Update()
    {
        // Show or hide all tracked AR planes
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(placementEnabled);
            Debug.Log("placementEnabled");
        }

        if (!placementEnabled || EnhancedTouch.Touch.activeTouches.Count == 0)
            return;

        Vector2 touchPosition = EnhancedTouch.Touch.activeTouches[0].screenPosition;

        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            ARPlane plane = planeManager.GetPlane(hits[0].trackableId);
            ARAnchor anchor = anchorManager.AttachAnchor(plane, hitPose);

            Vector3 spawnPos = hitPose.position + Vector3.up * spawnHeight;
            GameObject placedObject = Instantiate(selectedPrefab, spawnPos, hitPose.rotation, anchor.transform);

            if (selectedPrefab.CompareTag("Fairy"))
            {
                var rb = placedObject.GetComponent<Rigidbody>();
                if (rb != null) rb.useGravity = false;

                FairyFlyer flyer = placedObject.GetComponent<FairyFlyer>();
                if (flyer == null)
                    flyer = placedObject.AddComponent<FairyFlyer>();
            }

            Debug.Log($"Placed object at: {spawnPos}");
            placementEnabled = false;
            buttonManager.setAlternateView(0);
        }
    }

    /// <summary>
    /// Checks if the given screen position is over a UI element
    /// </summary>
    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
