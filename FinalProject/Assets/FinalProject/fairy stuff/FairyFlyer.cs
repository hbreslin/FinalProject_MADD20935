using UnityEngine;
using System.Collections;

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
    public float leafSpawnInterval = 0.5f;

    public float leafSpawnOffset = 0.1f;
    public float leafSpawnForwardOff = 0.3f;

    // House interaction variables
    public float houseDetectRange = .5f;
    public float approachDistance = 0.1f;
    public float pauseAtHouseDuration = 1f;
    public float minReappearDelay = 1f;
    public float maxReappearDelay = 2.5f;

    // Flocking variables
    public float flockNeighborRadius = 2.0f;       // Radius to detect neighbors
    public float flockAvoidanceRadius = 0.5f;      // Minimum distance to avoid crowding
    public float flockCohesionWeight = 1.0f;       // How strongly to move toward center of neighbors
    public float flockAlignmentWeight = 1.0f;      // How strongly to align direction with neighbors
    public float flockSeparationWeight = 1.5f;     // How strongly to avoid neighbors

    private bool isHouseInteracting = false;
    private float lastHouseExitTime = -999f;

    private Vector3 _direction;
    private Vector3 _targetDirection;
    private bool _isPaused;
    private float _stateTimer;
    private float _stateDuration;
    private float _leafTimer;
    private Vector3 _spawnCenter;
    private Vector3 _spawnEuler;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Start()
    {
        _spawnCenter = transform.position;
        _spawnEuler = transform.eulerAngles;
        BeginMovement();
    }

    void Update()
    {
        if (isHouseInteracting) return;

        float dt = Time.deltaTime;
        _stateTimer += dt;

        // House interaction logic
        if (Time.time - lastHouseExitTime > 2f) // Cooldown after visiting a house
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, houseDetectRange);
            foreach (var col in hits)
            {
                if (col.CompareTag("House"))
                {
                    StartCoroutine(VisitHouse(col.transform));
                    return;
                }
            }
        }

        // Wandering and leaf logic
        if (_stateTimer >= _stateDuration)
        {
            if (_isPaused) BeginMovement();
            else BeginPause();
        }

        if (!_isPaused)
        {
            // --- Flocking behavior ---
            Vector3 cohesion = Vector3.zero;
            Vector3 alignment = Vector3.zero;
            Vector3 separation = Vector3.zero;
            int neighborCount = 0;

            Collider[] neighbors = Physics.OverlapSphere(transform.position, flockNeighborRadius);
            foreach (var neighbor in neighbors)
            {
                if (neighbor.gameObject == gameObject) continue; // Skip self
                if (!neighbor.CompareTag("Fairy")) continue;     // Only other fairies

                Vector3 toNeighbor = neighbor.transform.position - transform.position;
                float dist = toNeighbor.magnitude;

                cohesion += neighbor.transform.position;
                alignment += neighbor.transform.forward;

                if (dist < flockAvoidanceRadius)
                {
                    // Move away to avoid crowding
                    separation -= (toNeighbor / dist) / dist;  // stronger repulsion when closer
                }

                neighborCount++;
            }

            if (neighborCount > 0)
            {
                cohesion = (cohesion / neighborCount - transform.position).normalized * flockCohesionWeight;
                alignment = (alignment / neighborCount).normalized * flockAlignmentWeight;
                separation = separation.normalized * flockSeparationWeight;

                _targetDirection = (_targetDirection + cohesion + alignment + separation).normalized;
            }

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

    private IEnumerator VisitHouse(Transform house)
    {
        isHouseInteracting = true;

        // Approach
        Vector3 dir = (house.position - transform.position).normalized;
        while (Vector3.Distance(transform.position, house.position) > approachDistance)
        {
            transform.position += dir * speed * Time.deltaTime;
            yield return null;
        }

        // Pause
        yield return new WaitForSeconds(pauseAtHouseDuration);

        // Disappear
        SetVisible(false);

        // Wait randomly
        float delay = Random.Range(minReappearDelay, maxReappearDelay);
        yield return new WaitForSeconds(delay);

        // Reappear next to house
        Vector3 spawnOffset = (house.forward * approachDistance) + (Vector3.up * 0.1f);
        transform.position = house.position + spawnOffset;
        SetVisible(true);

        // Fly away
        Vector3 away = (transform.position - house.position).normalized;
        float fleeDuration = 1.0f;
        float t = 0f;
        while (t < fleeDuration)
        {
            transform.position += away * speed * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }

        lastHouseExitTime = Time.time;
        isHouseInteracting = false;
    }

    private void SetVisible(bool on)
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = on;
        if (animator) animator.enabled = on;
    }

    void BeginMovement()
    {
        _isPaused = false;
        _stateTimer = 0f;
        _stateDuration = Random.Range(minSegmentDistance, maxSegmentDistance) / speed;

        animator.SetBool("Fly Forward", true);

        _direction = transform.forward;
        _targetDirection = Random.onUnitSphere;
        _leafTimer = 0f;
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

        Vector3 forward = transform.forward.normalized;
        Vector3 spawnPos = transform.position
                         + forward * leafSpawnForwardOff
                         + Vector3.up * leafSpawnOffset;

        Instantiate(leafPrefab, spawnPos, Quaternion.identity);
    }
}
