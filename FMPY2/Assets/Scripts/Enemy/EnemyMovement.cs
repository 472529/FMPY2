using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] public float enemyHealth = 100;
    [SerializeField] Transform target;
    [SerializeField] float rotationalDamp = 0.5f;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] Laser laser;
    [SerializeField] VisualEffect explosion;
    Vector3 hitPos;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("PlayerShip").transform;
        explosion = GetComponentInChildren<VisualEffect>();
    }
    private void Update()
    {
        Turn();
        Move();
        InFront();
        HaveLineOfSight();
        if(InFront() && HaveLineOfSight())
        {
            FireLaser();
            
        }
        Death();
    }

    

    private void Turn()
    {
        Vector3 pos = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationalDamp * Time.deltaTime);
    }

    private void Move()
    {
        transform.position += transform.forward* moveSpeed * Time.deltaTime;
    }

    bool InFront()
    {
        Vector3 dirTarget = transform.position - target.position;
        float angle = Vector3.Angle(transform.forward, dirTarget);

        if (Mathf.Abs(angle) > 90 && Mathf.Abs(angle) < 270)
        {
            Debug.DrawLine(transform.position, target.position, Color.green);
            return true;
        }
        Debug.DrawLine(transform.position, target.position, Color.red);
        return false;
    }

    bool HaveLineOfSight()
    {
        RaycastHit hit;
        Vector3 dir = target.position - transform.position;
        //Debug.DrawRay(laser.transform.position, dir, Color.blue);
        if (Physics.Raycast(laser.transform.position, dir, out hit, laser.EnemyLaserRange))
        {
            if (hit.transform.CompareTag("PlayerShip"))
            {
                Debug.DrawRay(laser.transform.position, dir, Color.blue);
                hitPos = hit.transform.position;
                return true;
            }
        }
        return false;
    }

    void FireLaser()
    {
        laser.FireLaser(hitPos);
    }

    void Death()
    {
        if (enemyHealth <= 0)
        {
            Instantiate(explosion.gameObject, this.transform);
            Destroy(this.gameObject);
        }
    }
}
