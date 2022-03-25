using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float rotationalDamp = 0.5f;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] Laser laser;

    Vector3 hitPos;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
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
}
