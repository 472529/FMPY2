using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpaceShipController : MonoBehaviour
{
    [Header("--- Ship Movement Settings ---")]
    [SerializeField]
    private float yawTorque = 500f;
    [SerializeField]
    private float pitchTorque = 1000f;
    [SerializeField]
    private float rollTorque = 1000f;
    [SerializeField]
    private float thrust = 50f;
    [SerializeField]
    private float upThrust = 50f;
    [SerializeField]
    private float strafeThrust = 50f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float thrustGlideReduction = 0.999f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float upDownGlideReduction = 0.111f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float leftRightthrustGlideReduction = 0.111f;

    Rigidbody rb;

    private float thrustID;
    private float upDownID;
    private float strafeID;
    private float rollID;
    private Vector2 pitchYaw;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {

    }

    #region Input Methods
    public void OnThrust(InputAction.CallbackContext context)
    {
        thrustID = context.ReadValue<float>();
    }
    public void OnThrust(InputAction.CallbackContext context)
    {
        strafeID = context.ReadValue<float>();
    }
    public void OnThrust(InputAction.CallbackContext context)
    {
        upDownID = context.ReadValue<float>();
    }
    public void OnThrust(InputAction.CallbackContext context)
    {
        rollID = context.ReadValue<float>();
    }
    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<float>();
    }
    #endregion
}
