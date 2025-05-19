using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARFairySpawner : MonoBehaviour
{
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] ARPlaneManager   planeManager;
    [SerializeField] ARAnchorManager  anchorManager;
    [SerializeField] GameObject       fairyPrefab;
    [SerializeField] float            spawnHeight = 0.15f;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        if (Input.touchCount == 0) return;
        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            var hit = hits[0];
            var plane = planeManager.GetPlane(hit.trackableId);
            var anchor = anchorManager.AttachAnchor(plane, hit.pose);

            Vector3 spawnPos = hit.pose.position + Vector3.up * spawnHeight;
            var fairy = Instantiate(
                fairyPrefab,
                spawnPos,
                Quaternion.identity,
                anchor.transform
            );

            // disable gravity if you have a Rigidbody
            var rb = fairy.GetComponent<Rigidbody>();
            if (rb != null) rb.useGravity = false;
            
            FairyFlyer flyer = fairy.GetComponent<FairyFlyer>();
            if (flyer == null)
                flyer = fairy.AddComponent<FairyFlyer>();
        }
    }
}
