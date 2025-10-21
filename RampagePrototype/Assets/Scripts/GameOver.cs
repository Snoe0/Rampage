using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    void Start()
    {
        HideGameOver();
    }

    public void ShowGameOver()
    {
        canvas.enabled = true;
    }

    public void HideGameOver()
    {
        canvas.enabled = false;
    }
}
