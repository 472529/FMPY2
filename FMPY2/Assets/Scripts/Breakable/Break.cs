using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : MonoBehaviour, IDamageable
{
    public float objectHealth = 100f;

    public void Damage(int damageAmount)
    {
        objectHealth -= damageAmount;
        if(objectHealth <= 0)
        {
            Destroy(this);
        }
    }


}
