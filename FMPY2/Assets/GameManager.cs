using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float Score;
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = ("Score: " + Score);
    }
}
