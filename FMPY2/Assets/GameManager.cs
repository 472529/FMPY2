using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float Score;
    public TextMeshProUGUI text;
    public SpaceShipGuns player;

    public float wave;
    public TextMeshProUGUI waveText;
    public WaveSpawner waveSpawner;
    public GameObject deathUI;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<SpaceShipGuns>();
        text = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMeshProUGUI>();
        waveText = GameObject.FindGameObjectWithTag("Wave").GetComponent<TextMeshProUGUI>();
        waveSpawner = GetComponent<WaveSpawner>();
        deathUI = GameObject.FindGameObjectWithTag("DeathUI");
        deathUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        wave = waveSpawner.currWave;
        text.text = ("Score: " + Score);
        waveText.text = ("Wave: " + wave);

        if (player.playerDead)
        {
            deathUI.SetActive(true); 
        }
    }

    
}
