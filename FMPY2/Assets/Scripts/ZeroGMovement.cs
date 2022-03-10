using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;


[RequireComponent(typeof(Rigidbody))]
public class ZeroGMovement : MonoBehaviour
{
    [Header("--- Player Movement Settings ---")]
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

    private Camera mainCam;
    [SerializeField]
    private CinemachineVirtualCamera playerCamera;

    [Header("--- Player Boost Settings ---")]
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

    private float thrust1D;
    private float upDown1D;
    private float strafe1D;
    private float rollID;

    public SpaceShipController ShipToEnter;

    public delegate void OnRequestShipEntry();
    public event OnRequestShipEntry onRequestShipEntry;

    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        currentBoostAmount = maxBoostAmount;
        ShipToEnter = null;
        

    }

    private void OnEnable()
    {
        if (playerCamera != null)
        {
            CameraSwitch.Register(playerCamera);
        }
        else
        {
            Debug.LogError("Player camera not assigned");
        }
    }

    private void OnDisable()
    {
        if (playerCamera != null) { CameraSwitch.UnRegister(playerCamera); }
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleBoosting();
    }

    public void AssignShip(SpaceShipController spaceship)
    {
        ShipToEnter = spaceship;
        if(ShipToEnter != null) { ShipToEnter.onRequestShipExit += ExitShip; }
    }

    public void UnassignShip()
    {
        ShipToEnter.onRequestShipExit -= ExitShip;
        ShipToEnter = null;  
    }

    void EnterShip()
    {
        transform.parent = ShipToEnter.transform;
        this.gameObject.SetActive(false);
        ShipToEnter.GetComponent<SpaceShipController>().enabled = true;

        if(onRequestShipEntry != null) { onRequestShipEntry(); }
       
    }

    void ExitShip()
    {
        transform.parent = ShipToEnter.transform;
        this.gameObject.SetActive(true);
        CameraSwitch.SwitchCamera(playerCamera);
        ShipToEnter.GetComponent<SpaceShipController>().enabled = false;
    }

    private void HandleBoosting()
    {
        if (boosting && currentBoostAmount > 0f)
        {
            currentBoostAmount -= boostDeprecationRate;
            if (currentBoostAmount <= 0f)
            {
                boosting = false;
            }
        }
        else
        {
            if (currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostRechageRate;
            }
        }
    }

    void HandleMovement()
    {
        //Roll
        rb.AddTorque(-mainCam.transform.forward * rollID * rollTorque * Time.deltaTime);

        //Thurst
        if (thrust1D > 0.1f || thrust1D < -0.1f)
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

            rb.AddForce(mainCam.transform.forward * thrust1D * currentThrust * Time.deltaTime);
            glide = thrust;
        }
        else
        {
            rb.AddForce(mainCam.transform.forward* glide * Time.deltaTime);
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
        if (strafe1D > 0.1f || strafe1D < -0.1f)
        {
            rb.AddForce(mainCam.transform.right * strafe1D * upThrust * Time.fixedDeltaTime);
            horizontalGlide = strafe1D * strafeThrust;
        }
        else
        {
            rb.AddForce(mainCam.transform.right * horizontalGlide * Time.fixedDeltaTime);
            horizontalGlide *= leftRightthrustGlideReduction;
        }
    }

    #region Input Methods
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
    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (ShipToEnter != null && context.action.triggered)
        {
            EnterShip();
        }
    }
    #endregion
}