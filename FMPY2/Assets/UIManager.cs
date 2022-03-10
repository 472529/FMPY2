using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image laserHeatImage;

    [SerializeField] private SpaceShipGuns shooting;

    private void Start()
    {
        shooting = FindObjectOfType<SpaceShipGuns>();
    }

    private void Update()
    {
        if(shooting != null)
        {
            laserHeatImage.fillAmount = shooting.CurrentLaserHeat / shooting.LaserHeatThreshold;
        }
    }
}
