using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    [SerializeField] float laserOnTime = 1f;
    [SerializeField] float laserRange = 100f;
    [SerializeField] LineRenderer lr;
    [SerializeField] float fireDelay = 2f;
    [SerializeField] SpaceShipGuns player;
    public float laserDmg;
    bool canFire;

    public float EnemyLaserRange { get { return laserRange; } }

    private void Start()
    {
        lr.enabled = false;
        canFire = true;
        player = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<SpaceShipGuns>();
    }

    Vector3 CastRay()
    {
        RaycastHit hit;

        Vector3 fwd = transform.TransformDirection(Vector3.forward) * laserRange;

        if(Physics.Raycast(transform.position, fwd, out hit))
        {
            Debug.Log("Hit: " + hit.transform.name);
            return hit.point;
        }
            Debug.Log("Missed");
        return transform.position + (transform.forward * laserRange);
    }

    

    public void FireLaser(Vector3 targetPos)
    {
        if (canFire)
        {
            lr.enabled = true;
            lr.SetPosition(1, this.transform.position);
            lr.SetPosition(0, targetPos);
            canFire = false;
            player.playerHealth -= laserDmg;
            if(player.playerHealth <= 0)
            {
                player.Death();
            }
            Invoke("TurnOffLaser", laserOnTime);
            Invoke("CanFire", fireDelay);
        }
    }

    void TurnOffLaser()
    {
        lr.enabled = false;
    }

    float Distance()
    {
        return laserRange;
    }

    void CanFire()
    {
        canFire = true;
    }
}
