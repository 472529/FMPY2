using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class SpaceShipGuns : MonoBehaviour
{
    [Header("SpaceShip Settings")]
    [SerializeField]
    private SpaceShipController spaceship;

    [Header("--- Hardpoint Settings ---")]
    [SerializeField]
    private Transform[] hardpoints;
    [SerializeField]
    private Transform hardpointMiddle;
    [SerializeField]
    private LayerMask shootableMask;
    [SerializeField]
    private float hardpointRange = 100f;
    private bool targetInRange = false;

    [Header("--- Laser Settings ---")]
    [SerializeField]
    private LineRenderer[] lasers;
    [SerializeField]
    private ParticleSystem laserHitParticles;
    [SerializeField]
    private float attackPower = 10f;
    [SerializeField]
    private float laserheatThreshold = 2f;
    [SerializeField]
    private float laserHeatRate = 0.25f;
    [SerializeField]
    private float laserCoolRate = 0.5f;
    private float currentLaserHeat = 0f;
    private bool overHeated = false;

    private bool firing;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        spaceship = GetComponent<SpaceShipController>();
    }

    private void Update()
    {
        if(spaceship.isOccupied)
        {
            
        }
        HandleLaserFiring();
    }

    private void HandleLaserFiring()
    {
        if (firing && !overHeated)
        {
            FireLaser();
        }
        else
        {
            foreach(var laser in lasers)
            {
                laser.gameObject.SetActive(false);
            }

            CoolLaser();
        }
    }

    private void CoolLaser()
    {
        
    }

    private void FireLaser()
    {
        RaycastHit hitInfo;

        if (TargetInfo.isTargetInRange(hardpointMiddle.transform.position, hardpointMiddle.transform.forward, out hitInfo, hardpointRange, shootableMask))
        {
            foreach(var laser in lasers)
            {
                Vector3 localHitPosition = laser.transform.InverseTransformPoint(hitInfo.point);
                laser.gameObject.SetActive(true);
                laser.SetPosition(1, localHitPosition);
            }
        }
        else
        {
            foreach(var laser in lasers)
            {
                laser.gameObject.SetActive(true);
                laser.SetPosition(1, new Vector3(0, 0, hardpointRange));
            }
        }
    }

    #region input
    public void OnFire(InputAction.CallbackContext context)
    {
        firing = context.performed;
    }
    #endregion
}
