using System.Collections.Generic;
using UnityEngine;

public class PlacedObjectManager : MonoBehaviour
{
    public static PlacedObjectManager Instance;

    public List<GameObject> placedObjects = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register(GameObject obj)
    {
        placedObjects.Add(obj);
    }
}
