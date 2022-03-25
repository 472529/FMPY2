using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float rotationalDamp = 0.5f;
    [SerializeField] float moveSpeed = 10f;
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        Turn();
        Move();
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
}
