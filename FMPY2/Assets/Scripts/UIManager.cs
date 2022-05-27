using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image laserHeatImage;
    [SerializeField] private Image boostHeatImage;
    [SerializeField] private Image warpHeatImage;
    [SerializeField] private Image healthImage;

    [SerializeField] private SpaceShipGuns shooting;
    [SerializeField] private SpaceShipController spaceship;

    private void Start()
    {
        shooting = FindObjectOfType<SpaceShipGuns>();
        spaceship = FindObjectOfType<SpaceShipController>();
    }

    private void Update()
    {
        if(shooting != null)
        {
            laserHeatImage.fillAmount = shooting.CurrentLaserHeat / shooting.LaserHeatThreshold;
        }
        if (spaceship != null)
        {
            boostHeatImage.fillAmount = spaceship.CurrentBoostAmount / spaceship.MaxBoostAmount;
            warpHeatImage.fillAmount = spaceship.CurrentWarpAmount / spaceship.MaxWarpAmount;
        }

        healthImage.fillAmount = shooting.playerHealth / shooting.maxPlayerHealth;
    }
}
