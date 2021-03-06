using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    private float maxBoostAmount = 15f;
    [SerializeField]
    private float boostDeprecationRate = 0.25f;
    [SerializeField]
    private float boostRechageRate = 0.5f;
    [SerializeField]
    private float boostMultiplyer = 5f;

    private bool boosting = false;
    private float currentBoostAmount;

    [Header("--- Ship Warp Settings ---")]
    [SerializeField]
    private float maxWarpAmount = 30f;
    [SerializeField]
    private float warpDeprecationRate = 0.5f;
    [SerializeField]
    private float warpRechageRate = 0.3f;
    [SerializeField]
    private float warpMultiplyer = 10f;



    private bool warping = false;
    private float currentWarpAmount;

    Rigidbody rb;

    [SerializeField] private CinemachineVirtualCamera shipCam;
    [SerializeField] private CinemachineVirtualCamera shipCamFPS;
    private float thrust1D;
    private float upDown1D;
    private float strafe1D;
    private float rollID;
    private Vector2 pitchYaw;

    private bool IsOccupied = false;

    public bool isOccupied { get { return IsOccupied; } }

    public float CurrentBoostAmount { get { return currentBoostAmount; } }
    public float MaxBoostAmount { get { return maxBoostAmount; } }
    public float CurrentWarpAmount { get { return currentWarpAmount; } }
    public float MaxWarpAmount { get { return maxWarpAmount; } }

    public ZeroGMovement player;

    public delegate void OnRequestShipExit();
    public event OnRequestShipExit onRequestShipExit;

    public VisualEffect warpSpeedVFX;
    public MeshRenderer warpCone;
    private bool warpActive;
    public float rate = 0.02f;
    public float delay = 3f;
    

    [SerializeField]
    private VolumeProfile volume;
    ChromaticAberration chromaticAberration;

    public GameObject lasers;
    public bool Inverted = true;

    public Mainmenu mm;
    public SpaceShipGuns guns;


    void Start()
    {
        mm = GameObject.FindGameObjectWithTag("UI").GetComponent<Mainmenu>();
        guns = GetComponent<SpaceShipGuns>();
        Inverted = mm.inverted;
        volume.TryGet<ChromaticAberration>(out chromaticAberration);
        chromaticAberration.intensity.value = 0;
        warpSpeedVFX.Stop();
        warpSpeedVFX.SetFloat("WarpAmount", 0);
        warpCone.material.SetFloat("Active_", 0);
        rb = GetComponent<Rigidbody>();
        currentBoostAmount = maxBoostAmount;
        currentWarpAmount = maxWarpAmount;
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<ZeroGMovement>();
        //if (player != null) { print("Player Found"); }
        lasers = GameObject.FindGameObjectWithTag("Lasers");
        lasers.SetActive(false);
        //player.onRequestShipEntry += EnterShip;
        EnterShip();
        //player.gameObject.SetActive(false);
    }

    //private void TurnToTarget(float x, float y, float z)
    //{
    //    Vector3 desiredHeading = new Vector3(x, y, z);

    //    Quaternion rotationGoal = Quaternion.LookRotation(desiredHeading);

    //    float step = yawTorque * Time.deltaTime;

    //    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationGoal, step);
    //}

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
        if (shipCam != null) { CameraSwitch.UnRegister(shipCam); }
    }
    void FixedUpdate()
    {
        if (IsOccupied && !guns.playerDead)
        {
            HandleMovement();
            HandleBoosting();
            HandleWarping();
        }
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

    private void HandleWarping()
    {
        if (warping && currentWarpAmount > 0f)
        {
            currentWarpAmount -= warpDeprecationRate;
            if (currentWarpAmount <= 0f)
            {
                warping = false;
            }
        }
        else
        {
            if (currentWarpAmount < maxWarpAmount)
            {
                currentWarpAmount += warpRechageRate;
            }
        }
    }

    void HandleMovement()
    {
        //Roll
        rb.AddRelativeTorque(Vector3.back * rollID * rollTorque * Time.deltaTime);
        //Pitch
        if (Inverted)
        {
            rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime);
        }
        else
        {
            rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(pitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime);
        }
        //Yaw 
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);

        //Thurst
        if (thrust1D > 0.1f || thrust1D < -0.1f)
        {
            float currentThrust;

            if (boosting)
            {
                currentThrust = thrust * boostMultiplyer;
            }
            else if (warping)
            {
                currentThrust = thrust * warpMultiplyer;
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
        if (strafe1D > 0.1f || strafe1D < -0.1f)
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
        lasers.SetActive(true);
    }

    void ExitShip()
    {
        rb.isKinematic = true;
        IsOccupied = false;
        lasers.SetActive(false);
        if (onRequestShipExit != null)
        {
            onRequestShipExit();
        }
    }

    IEnumerator ActivateParticles()
    {
        
        if (warpActive && currentWarpAmount > 0)
        {
            warpSpeedVFX.Play();

            volume.TryGet<ChromaticAberration>(out chromaticAberration);
            float amount = warpSpeedVFX.GetFloat("WarpAmount");
            float amount1 = chromaticAberration.intensity.value;

            while (amount < 1 && amount1 < 1 && warpActive)
            {
                amount += rate;
                amount1 += rate;
                warpSpeedVFX.SetFloat("WarpAmount", amount);
                chromaticAberration.intensity.value = amount1;
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            volume.TryGet<ChromaticAberration>(out chromaticAberration);
            float amount = warpSpeedVFX.GetFloat("WarpAmount");
            float amount1 = chromaticAberration.intensity.value;
            while (amount > 0 && amount1 > 0 && !warpActive && currentWarpAmount < maxWarpAmount)
            {
                amount -= rate;
                warpSpeedVFX.SetFloat("WarpAmount", amount);
                yield return new WaitForSeconds(0.1f);

                if (amount <= 0 + rate)
                {
                    amount = 0;
                    amount1 = 0;
                    warpSpeedVFX.SetFloat("WarpAmount", amount);
                    chromaticAberration.intensity.value = amount1;
                    warpActive = false;
                    warpSpeedVFX.Stop();
                }
            }

        }
    }

    IEnumerator ActivateShader()
    {

        if (warpActive && currentWarpAmount > 0)
        {
            rollTorque = 0f;
            pitchTorque = 100f;
            yawTorque = 100f;
            yield return new WaitForSeconds(delay);
            float amount = warpCone.material.GetFloat("Active_");
            while (amount < 1 && warpActive)
            {
                amount += rate;
                warpCone.material.SetFloat("Active_", amount);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            
            float amount = warpCone.material.GetFloat("Active_");
            while (amount > 0 && !warpActive && currentWarpAmount < maxWarpAmount)
            {
                amount -= rate;
                warpCone.material.SetFloat("Active_", amount);
                yield return new WaitForSeconds(0.1f);

                if (amount <= 0 + rate)
                {
                    rollTorque = 250f;
                    pitchTorque = 1000f;
                    yawTorque = 1000f;
                    amount = 0;
                    warpCone.material.SetFloat("Active_", amount);
                }
            }

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

    public void OnWarp(InputAction.CallbackContext context)
    {
        warping = context.performed;

        if (warping && IsOccupied)
        {
            warpActive = true;
            StartCoroutine(ActivateShader());
            StartCoroutine(ActivateParticles());

        }
        else
        {
            warpActive = false;
            StartCoroutine(ActivateShader());
            StartCoroutine(ActivateParticles());

        }
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
