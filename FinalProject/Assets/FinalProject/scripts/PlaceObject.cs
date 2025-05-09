using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using System;


[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlaceObject : MonoBehaviour{


    
}


// [RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
// public class PlaceObject : MonoBehaviour
// {
//     [SerializeField]
//     private List<GameObject> prefabs; // List of prefabs to choose from
//     [SerializeField]
//     private string enabledScene;

//     private ARRaycastManager aRRaycastManager;
//     private ARPlaneManager aRPlaneManager;
//     private List<ARRaycastHit> hits = new List<ARRaycastHit>();
//     private List<GameObject> placedObjects = new List<GameObject>(); // Track placed objects

//     private bool isTouching = false;
//     private float placementInterval = 0.2f;
//     private float lastPlacementTime = 0f;

//     private void Awake()
//     {
//         aRPlaneManager = GetComponent<ARPlaneManager>();
//         aRRaycastManager = GetComponent<ARRaycastManager>();
//     }

//     private void OnEnable()
//     {
//         EnhancedTouch.TouchSimulation.Enable();
//         EnhancedTouch.EnhancedTouchSupport.Enable();
//         EnhancedTouch.Touch.onFingerDown += OnFingerDown;
//         EnhancedTouch.Touch.onFingerUp += OnFingerUp;
//     }

//     private void OnDisable()
//     {
//         EnhancedTouch.TouchSimulation.Disable();
//         EnhancedTouch.EnhancedTouchSupport.Disable();
//         EnhancedTouch.Touch.onFingerDown -= OnFingerDown;
//         EnhancedTouch.Touch.onFingerUp -= OnFingerUp;
//     }

//     private void OnFingerDown(EnhancedTouch.Finger finger)
//     {
//         if (finger.index != 0)
//             return;

//         if (IsPointerOverUI(finger.currentTouch.screenPosition))
//             return;

//         isTouching = true;
//     }

//     private void OnFingerUp(EnhancedTouch.Finger finger)
//     {
//         if (finger.index == 0)
//             isTouching = false;
//     }

//     private void Update()
//     {
//         Scene currentScene = SceneManager.GetActiveScene();
//         bool shouldShowPlanes = currentScene.name == enabledScene;

//         // Show/hide all tracked AR planes
//         foreach (var plane in aRPlaneManager.trackables)
//         {
//             plane.gameObject.SetActive(shouldShowPlanes);
//         }

//         if (!shouldShowPlanes || !isTouching)
//             return;

//         if (Time.time - lastPlacementTime < placementInterval)
//             return;

//         if (EnhancedTouch.Touch.activeTouches.Count == 0)
//             return;

//         Vector2 touchPosition = EnhancedTouch.Touch.activeTouches[0].screenPosition;

//         if (aRRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
//         {
//             Pose pose = hits[0].pose;
//             Vector3 position = pose.position;

//             // Apply a small vertical offset to avoid overlapping
//             position.y += 0.1f;

//             // Choose a random prefab from the list
//             GameObject selectedPrefab = prefabs[UnityEngine.Random.Range(0, prefabs.Count)];
//             GameObject obj = Instantiate(selectedPrefab, position, pose.rotation);
//             DontDestroyOnLoad(obj); // Make object persist between scenes
//             placedObjects.Add(obj); // Track the object
//             lastPlacementTime = Time.time;

//             Debug.Log($"Placed object at: {position}");

//             // Add small horizontal offset to avoid exact overlap
//             Vector3 offset = new Vector3(0.1f * UnityEngine.Random.Range(1, 3), 0, 0);
//             obj.transform.position += offset;

//             // Rotate to face the camera
//             var plane = aRPlaneManager.GetPlane(hits[0].trackableId);
//             if (plane.alignment == PlaneAlignment.HorizontalUp)
//             {
//                 Vector3 cameraPosition = Camera.main.transform.position;
//                 cameraPosition.y = 0f;

//                 Vector3 direction = cameraPosition - position;
//                 obj.transform.rotation = Quaternion.LookRotation(direction);
//             }
//         }
//     }

//     /// <summary>
//     /// Checks if the given screen position is over a UI element
//     /// </summary>
//     private bool IsPointerOverUI(Vector2 screenPosition)
//     {
//         PointerEventData eventData = new PointerEventData(EventSystem.current)
//         {
//             position = screenPosition
//         };

//         List<RaycastResult> results = new List<RaycastResult>();
//         EventSystem.current.RaycastAll(eventData, results);
//         return results.Count > 0;
//     }

//     // Optional: Clear all placed objects
//     public void ClearPlacedObjects()
//     {
//         foreach (var obj in placedObjects)
//         {
//             Destroy(obj);
//         }
//         placedObjects.Clear();
//     }
// }
