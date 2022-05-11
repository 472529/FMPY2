using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class Death : MonoBehaviour
{
    EnemyMovement em;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Death(Vector3 position)
    {
        if (em.playerHealth <= 0)
        {
            gameObject.SetActive(false);
            VisualEffect explosion = em.explosion;
            Instantiate(explosion.gameObject, position, Quaternion.identity);
        }
    }
}
