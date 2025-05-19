using UnityEngine;

public class FairyFlyer : MonoBehaviour
{
    public Animator animator;
    public float speed = 0.5f;
    public float minSegmentDistance = 0.75f;
    public float maxSegmentDistance = 3.2f;
    public float minPauseDuration = 0.5f;
    public float maxPauseDuration = 2f;
    public float turnSpeed = 85f;
    public float boundaryRadius = 8f;
    public GameObject leafPrefab;
    public float    leafSpawnInterval = .5f;

    private Vector3 _direction;
    private Vector3 _targetDirection;
    private bool    _isPaused;
    private float   _stateTimer;
    private float   _stateDuration;
    private float   _leafTimer;
    private Vector3 _spawnCenter;
    private Vector3 _spawnEuler;
    public float    leafSpawnOffset   = 0.1f;
    public float    leafSpawnForwardOff = 0.3f;
    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Start()
    {
        _spawnCenter = transform.position;
        _spawnEuler  = transform.eulerAngles;
        BeginMovement();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        _stateTimer += dt;
        

        if (_stateTimer >= _stateDuration)
            if (_isPaused) BeginMovement();
            else          BeginPause();

        if (!_isPaused)
        {
            _direction = Vector3.RotateTowards(
                _direction,
                _targetDirection,
                turnSpeed * Mathf.Deg2Rad * dt,
                0f
            );

            Vector3 nextPos = transform.position + _direction * speed * dt;
            if ((nextPos - _spawnCenter).sqrMagnitude > boundaryRadius * boundaryRadius)
            {
                _targetDirection = (_spawnCenter - transform.position).normalized;
                _direction = Vector3.RotateTowards(
                    _direction,
                    _targetDirection,
                    turnSpeed * Mathf.Deg2Rad * dt,
                    0f
                );
            }

            transform.position += _direction * speed * dt;

            Vector3 flat = new Vector3(_direction.x, 0f, _direction.z);
            if (flat.sqrMagnitude > 0.001f)
            {
                float yaw = Mathf.Atan2(flat.x, flat.z) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(_spawnEuler.x, yaw, _spawnEuler.z);
            }
            _leafTimer += dt;
            if (_leafTimer >= leafSpawnInterval)
            {
                _leafTimer = 0f;
                SpawnLeaf();
            }
        }
    }

    void BeginMovement()
    {
        _isPaused = false;
        _stateTimer = 0f;
        _stateDuration = Random.Range(minSegmentDistance, maxSegmentDistance) / speed;

        animator.SetBool("Fly Forward", true);

        _direction = transform.forward;
        _targetDirection = Random.onUnitSphere;
        _leafTimer       = 0f;
    }

    void BeginPause()
    {
        _isPaused = true;
        _stateTimer = 0f;
        _stateDuration = Random.Range(minPauseDuration, maxPauseDuration);

        animator.SetBool("Fly Forward", false);
    }
    void SpawnLeaf()
    {
        if (leafPrefab == null) return;
        Vector3 forward  = transform.forward.normalized;
        Vector3 spawnPos = transform.position 
                     + forward * leafSpawnForwardOff
                     + Vector3.up * leafSpawnOffset;

        Instantiate(leafPrefab, spawnPos, Quaternion.identity);
    }
}