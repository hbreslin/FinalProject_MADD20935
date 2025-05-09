using UnityEngine;

public class ARSessionManager : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectsByType<ARSessionManager>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
