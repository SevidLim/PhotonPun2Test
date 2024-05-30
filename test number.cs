using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testnumber : MonoBehaviour
{
    public Text _text;
    public float score;

    void Start()
    {
        
    }

    void Update()
    {
        _text.text = score.ToString();
    }

    public void AddScore()
    {
        score += 1;
    }
}
