using System;
using UnityEngine;

public class AxisRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up; // Axis to rotate around
    public float rotationSpeed = 90f;         // Degrees per second
    public Space rotationSpace = Space.Self;  // Rotate in local or world space
    public bool physics;
    private Rigidbody _rb;
    private void Start()
    {
        if (physics) _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Normalize axis in case it's not unit length
        Vector3 axis = rotationAxis.normalized;

        if (physics) _rb.angularVelocity = axis * rotationSpeed;
        else transform.Rotate(axis, rotationSpeed * Time.deltaTime, rotationSpace);
    }
}