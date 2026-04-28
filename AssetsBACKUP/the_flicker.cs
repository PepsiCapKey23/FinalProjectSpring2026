using UnityEngine;

public class Flicker : MonoBehaviour
{
    [Header("Settings")]
    public float power = 10f;
    public float maxDragDistance = 5f;
    [Range(10, 100)] public float maxForceLimit = 50f;
    public int trajectorySteps = 50; 

    [Header("Reset Logic")]
    public Transform champSpawnPoint; 
    public float settleThreshold = 0.15f;

    [Header("Simple Audio")]
    public AudioSource flickSource;

    private Rigidbody2D rb;
    private Camera cam;
    private Vector3 startPoint;
    private LineRenderer lineRenderer; 
    private bool isAiming = false;
    private bool hasFlicked = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        HandleReset();

        if (rb.linearVelocity.magnitude > settleThreshold) return;

        HandleInput();
    }

    private void HandleReset()
    {
        if (hasFlicked && rb.linearVelocity.magnitude < settleThreshold)
        {
            ResetToStart();
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) StartAiming();
        if (Input.GetMouseButton(0) && isAiming) ContinueAiming();
        if (Input.GetMouseButtonUp(0) && isAiming) Shoot();
    }

    private void StartAiming()
    {
        startPoint = GetMouseWorldPos();
        isAiming = true;
    }

    private void ContinueAiming()
    {
        Vector2 dragVector = Vector2.ClampMagnitude((Vector2)startPoint - (Vector2)GetMouseWorldPos(), maxDragDistance);
        PlotTrajectory((Vector2)transform.position, dragVector * power);
    }

    private void Shoot()
    {
        Vector2 dragVector = Vector2.ClampMagnitude((Vector2)startPoint - (Vector2)GetMouseWorldPos(), maxDragDistance);
        Vector2 force = Vector2.ClampMagnitude(dragVector * power, maxForceLimit);

        rb.AddForce(force, ForceMode2D.Impulse);

        if (flickSource != null) flickSource.PlayOneShot(flickSource.clip);

        lineRenderer.enabled = false; 
        isAiming = false;
        hasFlicked = true;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        return pos;
    }

    public void PlotTrajectory(Vector2 pos, Vector2 velocity)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = trajectorySteps;
        Vector3[] points = new Vector3[trajectorySteps];
        
        float timestep = Time.fixedDeltaTime;
        float drag = 1f - timestep * rb.linearDamping; 
        Vector2 moveStep = velocity * timestep;

        for (int i = 0; i < trajectorySteps; i++)
        {
            moveStep *= drag;
            pos += moveStep;
            points[i] = new Vector3(pos.x, pos.y, 0);
        }

        lineRenderer.SetPositions(points);
    }

    void ResetToStart()
    {
        hasFlicked = false; 
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if(champSpawnPoint != null)
        {
            rb.position = champSpawnPoint.position;
            transform.position = champSpawnPoint.position;

            rb.WakeUp();
            Physics2D.SyncTransforms(); 
        }
    }
}