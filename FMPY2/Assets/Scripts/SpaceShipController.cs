using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

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
    private float glide, horizontalGlide, verticalGlide = 0f;

    [Header("--- Ship Boost Settings ---")]
    [SerializeField]
    private float maxBoostAmount = 2f;
    [SerializeField]
    private float boostDeprecationRate = 0.25f;
    [SerializeField]
    private float boostRechageRate = 0.5f;
    [SerializeField]
    private float boostMultiplyer = 5f;

    private bool boosting = false;
    private float currentBoostAmount;

    Rigidbody rb;

    [SerializeField] private CinemachineVirtualCamera shipCam;
    [SerializeField] private CinemachineVirtualCamera shipCamFPS;
    private float thrust1D;
    private float upDown1D;
    private float strafe1D;
    private float rollID;
    private Vector2 pitchYaw;

    private bool IsOccupied = false;

    public ZeroGMovement player;

    public delegate void OnRequestShipExit();
    public event OnRequestShipExit onRequestShipExit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentBoostAmount = maxBoostAmount;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<ZeroGMovement>();
        if(player != null) { print("Player Found"); }

        player.onRequestShipEntry += EnterShip;

    }

    private void OnEnable()
    {
        if (shipCam != null)
        {
            CameraSwitch.Register(shipCam);
        }
        else
        {
            Debug.LogError("Ship camera not assigned");
        }
    }

    private void OnDisable()
    {
        if(shipCam != null) { CameraSwitch.UnRegister(shipCam); }
    }
    void FixedUpdate()
    {
        if (IsOccupied)
        {
            HandleMovement();
            HandleBoosting();
        }
    }

    private void HandleBoosting()
    {
        if(boosting && currentBoostAmount > 0f)
        {
            currentBoostAmount -= boostDeprecationRate;
            if(currentBoostAmount <= 0f)
            {
                boosting = false;
            }
        }
        else
        {
            if(currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostRechageRate;
            }
        }
    }

    void HandleMovement()
    {
        //Roll
        rb.AddRelativeTorque(Vector3.back * rollID * rollTorque * Time.deltaTime);
        //Pitch
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(pitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime);
        //Yaw 
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);

        //Thurst
        if(thrust1D > 0.1f || thrust1D < -0.1f)
        {
            float currentThrust;

            if (boosting)
            {
                currentThrust = thrust * boostMultiplyer;
            }
            else
            {
                currentThrust = thrust;
            }

            rb.AddRelativeForce(Vector3.forward * thrust1D * currentThrust * Time.deltaTime);
            glide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= thrustGlideReduction;
        }

        //UpDown
        if (upDown1D > 0.1f || upDown1D < -0.1f)
        {
            rb.AddRelativeForce(Vector3.up * upDown1D * upThrust * Time.deltaTime);
            verticalGlide = upDown1D * upThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.up * verticalGlide * Time.fixedDeltaTime);
            verticalGlide *= upDownGlideReduction;
        }
        //Strafe
        if(strafe1D > 0.1f || strafe1D < -0.1f)
        {
            rb.AddRelativeForce(Vector3.right * strafe1D * upThrust * Time.fixedDeltaTime);
            horizontalGlide = strafe1D * strafeThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.fixedDeltaTime);
            horizontalGlide *= leftRightthrustGlideReduction; 
        }
    }

    void EnterShip()
    {
        rb.isKinematic = false;
        CameraSwitch.SwitchCamera(shipCam);
        IsOccupied = true;
    }

    void ExitShip()
    {
        rb.isKinematic = true;
        IsOccupied = false;
        if(onRequestShipExit != null)
        {
            onRequestShipExit();
        }
    }

    #region Input Methods

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (IsOccupied && context.action.triggered)
        {
            ExitShip();
        }
    }
    public void OnThrust(InputAction.CallbackContext context)
    {
        thrust1D = context.ReadValue<float>();
        
    }
    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1D = context.ReadValue<float>();
    }
    public void OnUpDown(InputAction.CallbackContext context)
    {
        upDown1D = context.ReadValue<float>();
    }
    public void OnRoll(InputAction.CallbackContext context)
    {
        rollID = context.ReadValue<float>();
    }
    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
    }
    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
    }

    public void OnSwitchCamera(InputAction.CallbackContext context)
    {
        if (IsOccupied && context.action.triggered)
        {
            if (CameraSwitch.IsActiveCamera(shipCamFPS))
            {
                CameraSwitch.SwitchCamera(shipCam);
            }
            else if (CameraSwitch.IsActiveCamera(shipCam))
            {
                CameraSwitch.SwitchCamera(shipCamFPS);
            }
        }
    }
    #endregion
}
