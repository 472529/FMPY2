using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateAsteroidField : MonoBehaviour
{
    [SerializeField] Transform asteroidPrefab;
    [SerializeField] int fieldRadius = 100;
    [SerializeField] int asteroidCount = 500;
    void Start()
    {
        for (int loop=0; loop <asteroidCount; loop++)
        {
            //index = Random.Range(0, asteroidPrefab.Length);
            //Game
            Instantiate(asteroidPrefab, Random.insideUnitSphere * fieldRadius, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
