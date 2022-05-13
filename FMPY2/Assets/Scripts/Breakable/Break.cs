using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : MonoBehaviour, IDamageable
{
    public float objectHealth = 100f;
    private Vector3 lastDamagePos;

    [SerializeField] private Transform brokenRock;
    //[SerializeField] private Transform vfxSmoke;
    
    

    public void Damage(int damageAmount, Vector3 damagePos)
    {
        lastDamagePos = damagePos;
        objectHealth -= damageAmount;
        if(objectHealth <= 0)
        {
            //Instantiate(vfxSmoke, lastDamagePos, Quaternion.identity);
            Transform rockBrokenTransform = Instantiate(brokenRock, transform.position, Quaternion.identity);
            foreach(Transform child in rockBrokenTransform)
            {
                if(child.TryGetComponent<Rigidbody>(out Rigidbody childRigidBody))
                {
                    childRigidBody.AddExplosionForce(100f, lastDamagePos, 5f);
                }
            }

            Destroy(this.gameObject);
        }
    }


}
