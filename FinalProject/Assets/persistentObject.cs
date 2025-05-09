using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    void Awake()
    {
        // Make this object persist between scenes
        DontDestroyOnLoad(gameObject);
    }
}
