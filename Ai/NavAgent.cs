using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Moving and turning operation.
/// </summary>
public class NavAgent : NetworkBehaviour
{
    // Speed im m/s
    private float _speed = 1f;
    // Can moving
    private bool _move = true;
    // Can turning
    private bool _turn = true;
    // Destination position

    [HideInInspector]
    [SyncVar]
    private Vector2 _destination;
    // Velocity vector
    [HideInInspector]
    private Vector2 _velocity;

    // Position on last frame
    private Vector2 _prevPosition;

    public Vector2 PrevPosition
    {
        get { return _prevPosition; } 
        set { _prevPosition = value; }
    }

    public Vector2 Velocity
    {
        get { return _velocity; }
        set { _velocity = value; }
    }

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    public bool Move
    {
        get { return _move; }
        set { _move = value; }
    }

    public bool Turn
    {
        get { return _turn; }
        set { _turn = value; }
    }

    public Vector2 Destination
    {
        get { return _destination; }
        set { _destination = value; }
    }

    /// <summary>
    /// Raises the enable event.
    /// </summary>
    void OnEnable()
    {
        PrevPosition = transform.position;
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update()
    {
        // If moving is allowed
        if (_move == true)
        {
            // Move towards destination point
            transform.position = Vector2.MoveTowards(transform.position, Destination, Speed * Time.deltaTime);
        }
        // Calculate velocity
        Vector2 velocity = (Vector2)transform.position - PrevPosition;
        velocity /= Time.deltaTime;
        // If turning is allowed
        if (Turn == true)
        {
            SetSpriteDirection(Destination - (Vector2)transform.position);
        }
        // Save last position
        PrevPosition = transform.position;
    }

    /// <summary>
    /// Sets sprite direction on x axis.
    /// </summary>
    /// <param name="direction">Direction.</param>
    private void SetSpriteDirection(Vector2 direction)
    {
        if (direction.x > 0f && transform.localScale.x < 0f) // To the right
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < 0f && transform.localScale.x > 0f) // To the left
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    /// <summary>
    /// Looks at direction.
    /// </summary>
    /// <param name="direction">Direction.</param>
    public void LookAt(Vector2 direction)
    {
        SetSpriteDirection(direction);
    }

    /// <summary>
    /// Looks at target.
    /// </summary>
    /// <param name="target">Target.</param>
    public void LookAt(Transform target)
    {
        SetSpriteDirection(target.position - transform.position);
    }
}
