using UnityEngine;
using System;

public class CollectableBobbing : MonoBehaviour
{
    private float                   f_startingY         = 0.0f;
    [SerializeField] private float  f_floatStrength     = 0.0f;
    [SerializeField] private float  f_floatBobSpeed     = 0.0f;
    [SerializeField] private float  f_floatRotateSpeed  = 0.0f;

    private void Start()
    {
        f_startingY
            = transform.position.y;
    }

    private void Update()
    {
        Bob();
        Rotate();
    }

    /// <summary>
    /// The Builder
    /// Moves the collectable up and down
    /// </summary>
    private void Bob()
    {
        transform.position
            = new Vector3
            (
                transform.position.x,
                f_startingY +
                ((float)Math.Sin(Time.time * f_floatBobSpeed) * f_floatStrength),
                transform.position.z
            );
    }

    /// <summary>
    /// Spins the object around
    /// These comments make me look more productive
    /// </summary>
    private void Rotate()
    {
        transform.Rotate
            (
                0.0f, (f_floatRotateSpeed * 10) * Time.deltaTime,
                0.0f, Space.Self
            );
    }
}
