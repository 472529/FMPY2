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
    [SerializeField] public Transform explosion;
    [SerializeField] float detectionDist = 20f;
    [SerializeField] float raycastOffset = 2.5f;
    public Vector3 hitPos;
    bool IsEnemyDead = false;
    [SerializeField] GameManager gm;
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("PlayerShip").transform;
        explosion = GameObject.FindGameObjectWithTag("Explosion").transform;
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        
    }
    private void Update()
    {
        PathFinding();
        Move();
        InFront();
        HaveLineOfSight();
        if(InFront() && HaveLineOfSight())
        {
            FireLaser();
            
        }
        if(IsEnemyDead)
        {
            Death(transform.position);
            IsEnemyDead = false;
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
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void PathFinding()
    {
        RaycastHit hit;
        Vector3 rayCastOffset = Vector3.zero;

        Vector3 left = transform.position - transform.right * raycastOffset;
        Vector3 right = transform.position + transform.right * raycastOffset;
        Vector3 up = transform.position + transform.up * raycastOffset;
        Vector3 down = transform.position - transform.up * raycastOffset;

        Debug.DrawRay(left, transform.forward * detectionDist, Color.cyan);
        Debug.DrawRay(right, transform.forward * detectionDist, Color.cyan);
        Debug.DrawRay(up, transform.forward * detectionDist, Color.cyan);
        Debug.DrawRay(down, transform.forward * detectionDist, Color.cyan);

        if (Physics.Raycast(left, transform.forward, out hit, detectionDist))
        {
            rayCastOffset += Vector3.right;
        }
        else if (Physics.Raycast(right, transform.forward, out hit, detectionDist))
        {
            rayCastOffset -= Vector3.right;
        }

        if (Physics.Raycast(up, transform.forward, out hit, detectionDist))
        {
            rayCastOffset -= Vector3.up;
        }
        else if (Physics.Raycast(down, transform.forward, out hit, detectionDist))
        {
            rayCastOffset += Vector3.up;
        }

        if (rayCastOffset != Vector3.zero)
        {
            transform.Rotate(rayCastOffset * 5f * Time.deltaTime);
        }
        else Turn();
    }

    bool InFront()
    {
        Vector3 dirTarget = transform.position - target.position;
        float angle = Vector3.Angle(transform.forward, dirTarget);

        if (Mathf.Abs(angle) > 90 && Mathf.Abs(angle) < 270)
        {
            Debug.DrawLine(transform.position, target.position, Color.blue);
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
                Debug.DrawRay(laser.transform.position, dir, Color.green);
                hitPos = hit.transform.GetChild(0).position;
                
                return true;
            }
        }
        return false;
    }

    void FireLaser()
    {
        laser.FireLaser(hitPos);
    }

    public void Death(Vector3 position)
    {
        IsEnemyDead = true;
        gm.Score += 100f; ;
        var Explosion = Instantiate(explosion.gameObject, position, Quaternion.identity);
        explosion.localScale = new Vector3(4, 4, 4);
        Destroy(this.gameObject);
        Destroy(Explosion, 1f);
        
    }
}
