using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedObjectData
{
    public string prefabName;
    public string tag;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class SavedObjectDataList
{
    public List<SavedObjectData> objects;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    // Tags to look for and save
    public List<string> tagsToSave = new List<string> { "Fairy", "House", "Decor" };

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
            ClearSavedData();
#endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadObjects();
    }

    public void SaveObjects()
    {
        List<SavedObjectData> dataList = new List<SavedObjectData>();

        foreach (string tag in tagsToSave)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject obj in objects)
            {
                // Ensure it's a prefab clone and not a scene object
                if (obj.scene.rootCount == 0) continue;

                string prefabName = obj.name.Replace("(Clone)", "").Trim();
                Debug.Log("saving prefab with name " + prefabName);

                dataList.Add(new SavedObjectData
                {
                    prefabName = prefabName,
                    tag = obj.tag,
                    position = obj.transform.position,
                    rotation = obj.transform.rotation
                });
            }
        }

        string json = JsonUtility.ToJson(new SavedObjectDataList { objects = dataList });
        PlayerPrefs.SetString("SavedSceneObjects", json);
        PlayerPrefs.Save();

        Debug.Log($"[SaveManager] Saved {dataList.Count} objects.");
    }

    public void LoadObjects()
    {
        if (!PlayerPrefs.HasKey("SavedSceneObjects")) return;

        string json = PlayerPrefs.GetString("SavedSceneObjects");
        SavedObjectDataList dataList = JsonUtility.FromJson<SavedObjectDataList>(json);

        foreach (var data in dataList.objects)
        {
            GameObject prefab = Resources.Load<GameObject>(data.prefabName);
            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab, data.position, data.rotation);
                obj.tag = data.tag;
            }
            else
            {
                Debug.LogWarning($"[SaveManager] Could not find prefab: {data.prefabName} in Resources.");
            }
        }

        Debug.Log($"[SaveManager] Loaded {dataList.objects.Count} objects.");
    }

    public void ClearSavedData()
    {
        if (PlayerPrefs.HasKey("SavedSceneObjects"))
        {
            PlayerPrefs.DeleteKey("SavedSceneObjects");
            PlayerPrefs.Save();
            Debug.Log("[SaveManager] Cleared all saved data.");
        }
        else
        {
            Debug.Log("[SaveManager] No saved data to clear.");
        }

        // Optionally destroy currently loaded saved objects in the scene
        foreach (string tag in tagsToSave)
        {
            GameObject[] existing = GameObject.FindGameObjectsWithTag(tag);
            foreach (var obj in existing)
            {
                Destroy(obj);
            }
        }
    }

    void OnApplicationQuit()
    {
        SaveObjects();
    }

    // Uncomment this if you want to save when app is paused (e.g. iOS background)
    // void OnApplicationPause(bool pause)
    // {
    //     if (pause) SaveObjects();
    // }
}
