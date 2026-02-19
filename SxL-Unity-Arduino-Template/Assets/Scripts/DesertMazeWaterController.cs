using UnityEngine;

/// <summary>
/// Attach to the water ball in the desert maze.
/// - Moves the ball using mouse delta (works when a wired Arduino joystick is used as a mouse).
/// - When the ball touches the fountain, triggers the wireless Arduino buzzer via DesertMazeGameManager.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DesertMazeWaterController : MonoBehaviour
{
    [Header("Movement (Joystick / Mouse)")]
    [Tooltip("Force applied per unit of mouse delta. Increase if the ball feels sluggish.")]
    public float moveForce = 120f;
    [Tooltip("Mouse delta is multiplied by this before applying force.")]
    public float sensitivity = 0.5f;
    [Tooltip("Movement plane: XZ = top-down; XY = side view. Use the plane that matches your maze.")]
    public MovementPlane movementPlane = MovementPlane.XZ;
    [Tooltip("Clamp horizontal speed so the ball does not go too fast.")]
    public float maxSpeed = 15f;

    [Header("Fountain (Win)")]
    [Tooltip("Name of the fountain GameObject (or its child). Used to detect collision.")]
    public string fountainObjectName = "fountain";

    private Rigidbody _rb;
    private bool _reachedFountain;

    public enum MovementPlane { XZ, XY }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
            _rb = gameObject.AddComponent<Rigidbody>();
        // Keep default drag a bit so the ball doesn’t slide forever
        _rb.linearDamping = 1f;
        _rb.angularDamping = 0.5f;
    }

    private void Update()
    {
        if (_reachedFountain) return;

        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        if (Mathf.Abs(mx) < 0.01f && Mathf.Abs(my) < 0.01f) return;

        Vector3 force = Vector3.zero;
        switch (movementPlane)
        {
            case MovementPlane.XZ:
                force = new Vector3(mx, 0f, my) * (sensitivity * moveForce);
                break;
            case MovementPlane.XY:
                force = new Vector3(mx, my, 0f) * (sensitivity * moveForce);
                break;
        }

        _rb.AddForce(force);
    }

    private void FixedUpdate()
    {
        if (_reachedFountain) return;

        // Soft speed clamp on the movement plane
        Vector3 vel = _rb.linearVelocity;
        if (movementPlane == MovementPlane.XZ)
        {
            float xz = new Vector2(vel.x, vel.z).magnitude;
            if (xz > maxSpeed)
            {
                float s = maxSpeed / xz;
                _rb.linearVelocity = new Vector3(vel.x * s, vel.y, vel.z * s);
            }
        }
        else
        {
            float xy = new Vector2(vel.x, vel.y).magnitude;
            if (xy > maxSpeed)
            {
                float s = maxSpeed / xy;
                _rb.linearVelocity = new Vector3(vel.x * s, vel.y * s, vel.z);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_reachedFountain) return;

        GameObject go = other.gameObject;
        if (IsFountain(go))
        {
            _reachedFountain = true;
            if (DesertMazeGameManager.Instance != null)
                DesertMazeGameManager.Instance.SendBuzzer();
            Debug.Log("Water reached the fountain! Buzzer sent to Arduino.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_reachedFountain) return;

        if (IsFountain(other.gameObject))
        {
            _reachedFountain = true;
            if (DesertMazeGameManager.Instance != null)
                DesertMazeGameManager.Instance.SendBuzzer();
            Debug.Log("Water reached the fountain! Buzzer sent to Arduino.");
        }
    }

    private bool IsFountain(GameObject go)
    {
        if (go == null) return false;
        if (string.Compare(go.name, fountainObjectName, true) == 0) return true;
        Transform p = go.transform.parent;
        while (p != null)
        {
            if (string.Compare(p.name, fountainObjectName, true) == 0) return true;
            p = p.parent;
        }
        return false;
    }
}
