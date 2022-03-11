using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpRotationLock : MonoBehaviour
{
    [SerializeField]
    private Transform warpTarget;
    [SerializeField]
    private Transform warp;

    // Start is called before the first frame update
    void Start()
    {
        warpTarget = GameObject.FindGameObjectWithTag("WarpTarget").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        warp.position = warpTarget.position;
    }
}
