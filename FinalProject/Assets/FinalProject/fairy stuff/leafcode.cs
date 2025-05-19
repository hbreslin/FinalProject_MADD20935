using UnityEngine;

public class LeafTrail : MonoBehaviour
{
    public float lifetime = 4.5f;
    private float _timer;
    // private Vector3 _startScale;

    // void Start()
    // {
    //     _startScale = transform.localScale;
    // }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= lifetime)
            Destroy(gameObject);
    }
}
