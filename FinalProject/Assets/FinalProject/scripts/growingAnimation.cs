using UnityEngine;

public class growingAnimation : MonoBehaviour
{
    public Vector3 targetScale = new Vector3(1f, 1f, 1f); // Final size
    public float growDuration = 2f; // Time in seconds to reach full size

    private Vector3 initialScale;
    private float timer = 0f;

    void Start()
    {
        initialScale = transform.localScale;
        transform.localScale = Vector3.zero; // Start from 0 scale (invisible)
    }

    void Update()
    {
        if (timer < growDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / growDuration);
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, progress);
        }
    }
}
