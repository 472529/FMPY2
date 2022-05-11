using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;


public class SpaceShipGuns : MonoBehaviour
{
    [Header("SpaceShip Settings")]
    [SerializeField]
    private SpaceShipController spaceship;
    [SerializeField]
    public float playerHealth = 100;

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
    private float laserHeatThreshold = 2f;
    [SerializeField]
    private float laserHeatRate = 0.25f;
    [SerializeField]
    private float laserCoolRate = 0.5f;
    private float currentLaserHeat = 0f;
    private bool overHeated = false;

    private bool firing;

    private Camera cam;

    public float CurrentLaserHeat { get { return currentLaserHeat; } }
    public float LaserHeatThreshold { get { return laserHeatThreshold; } }

    [SerializeField] GameObject enemies;
    [SerializeField] EnemyMovement em;
    [SerializeField] Death enemyDeath;
    

    private void Awake()
    {
        cam = Camera.main;
        spaceship = GetComponent<SpaceShipController>();
        
        
    }

    private void FixedUpdate()
    {
        if(spaceship.isOccupied)
        {
            HandleLaserFiring();
            
        }
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

    

    private void FireLaser()
    {
        RaycastHit hitInfo;

        if (TargetInfo.isTargetInRange(hardpointMiddle.transform.position, hardpointMiddle.transform.forward, out hitInfo, hardpointRange, shootableMask))
        {
            enemies = GameObject.FindGameObjectWithTag("EnemyShip");
            em = enemies.GetComponentInChildren<EnemyMovement>();
            em.enemyHealth -= attackPower;
            Instantiate(laserHitParticles, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

            foreach (var laser in lasers)
            {
                Vector3 localHitPosition = laser.transform.InverseTransformPoint(hitInfo.point);
                laser.gameObject.SetActive(true);
                laser.SetPosition(1, localHitPosition);
                if (em.enemyHealth <= 0)
                {
                    em.Death(em.transform.position);
                }
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
        HeatLaser();
    }

    private void HeatLaser()
    {
        if(firing && currentLaserHeat < laserHeatThreshold)
        {
            currentLaserHeat += laserHeatRate;

            if(currentLaserHeat >= laserHeatThreshold)
            {
                overHeated = true;
                firing = false;
            }
        }
    }

    private void CoolLaser()
    {
        if (overHeated)
        {
            if (currentLaserHeat / laserHeatThreshold <= 0.5f)
            {
                overHeated = false;
            }
        }
        if (currentLaserHeat > 0f)
        {
            currentLaserHeat -= laserCoolRate;
        }
    }

    

    #region input
    public void OnFire(InputAction.CallbackContext context)
    {
        firing = context.performed;
    }
    #endregion
}
