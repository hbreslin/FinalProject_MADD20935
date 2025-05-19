using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager), typeof(ARAnchorManager))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField] private GameObject buttonManagerObject;
    private NewButtonManager buttonManager;

    [SerializeField] private GameObject selectedPrefab;
    [SerializeField] private float spawnHeight = 0.15f;

    [Header("Message Objects")]
    [SerializeField] private GameObject tooManyFairiesMessage;
    [SerializeField] private GameObject sadFairiesMessage;
    [SerializeField] private float messageDuration = 2.5f;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private ARAnchorManager anchorManager;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool placementEnabled = false;
    private bool isTouching = false;

    private int houseCount = 0;
    private int fairyCount = 0;
    private int decorCount = 0;

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
        StartCoroutine(EnablePlacementWithDelay(0.5f));
    }

    private IEnumerator EnablePlacementWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
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
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(placementEnabled);
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

            // Check for fairy placement limit
            // Check for fairy placement limit
            if (selectedPrefab.CompareTag("Fairy") && fairyCount >= 2 * houseCount)
            {
                ShowMessage(tooManyFairiesMessage);
                buttonManager.setAlternateView(0); // Reset buttons
                placementEnabled = false;
                return;
            }


            GameObject placedObject = Instantiate(selectedPrefab, spawnPos, hitPose.rotation, anchor.transform);

            // Count objects based on tag
            if (selectedPrefab.CompareTag("Fairy"))
            {
                fairyCount++;

                var rb = placedObject.GetComponent<Rigidbody>();
                if (rb != null) rb.useGravity = false;

                FairyFlyer flyer = placedObject.GetComponent<FairyFlyer>();
                if (flyer == null)
                    flyer = placedObject.AddComponent<FairyFlyer>();
            }
            else if (selectedPrefab.CompareTag("House"))
            {
                houseCount++;
            }
            else if (selectedPrefab.CompareTag("Decor"))
            {
                decorCount++;
            }

            // Show sad fairies message if there's not enough decor
            if (houseCount > 0 && decorCount < houseCount / 2)
            {
                ShowMessage(sadFairiesMessage);
                buttonManager.setAlternateView(0); // Reset buttons
            }


            placementEnabled = false;
            buttonManager.setAlternateView(0);
        }
    }

    private void ShowMessage(GameObject messageObject)
    {
        if (messageObject == null) return;

        StartCoroutine(ShowMessageCoroutine(messageObject));
    }

    private IEnumerator ShowMessageCoroutine(GameObject message)
    {
        message.SetActive(true);
        yield return new WaitForSeconds(messageDuration);
        message.SetActive(false);
    }

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
