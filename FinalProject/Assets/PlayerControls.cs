using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private InputActionReference moveActionToUse;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] public GameObject trailPrefab; // ← Assign your 3D leaf prefab here
    [SerializeField] private float spawnInterval = 0.1f; // Minimum distance between objects

    private Vector3 lastSpawnPosition;

    void Start()
    {
        lastSpawnPosition = transform.position;

        // Spawn the first object immediately
        if (trailPrefab != null)
            Instantiate(trailPrefab, transform.position, Quaternion.identity);
    }

    void Update()
    {
        Vector2 input = moveActionToUse.action.ReadValue<Vector2>();

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = cameraRight * input.x + cameraForward * input.y;
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // Drop prefab if moved far enough
        if (Vector3.Distance(lastSpawnPosition, transform.position) >= spawnInterval)
        {
            if (trailPrefab != null)
            {
                Instantiate(trailPrefab, transform.position, Quaternion.identity);
                lastSpawnPosition = transform.position;
            }
        }
    }

    public void SetTrailPrefab(GameObject newPrefab)
    {
        trailPrefab = newPrefab;
    }
}
