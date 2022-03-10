using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEnterZone : MonoBehaviour
{
    [SerializeField]
    private SpaceShipController spaceship;

    private ZeroGMovement player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject.GetComponentInParent<ZeroGMovement>();
            if (player != null) player.AssignShip(spaceship);

            Debug.Log("Player In Enter Zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (player != null) player.UnassignShip();

            Debug.Log("Player Left Enter Zone");
        }
    }
}
