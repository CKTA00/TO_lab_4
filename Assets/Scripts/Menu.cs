using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    bool isPaused = false;
    public Button createButton;
    public Button readButton;
    public Population population;

    void Start()
    {
        population = FindObjectOfType<Population>();
    }

    public void PauseToggle()
    {
        isPaused = !isPaused;

        if(isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        createButton.interactable = isPaused;
        readButton.interactable = isPaused;
    }

    public void Create()
    {
        population.CreateSnapshot();
    }

    public void Read()
    {
        population.ReadSnapshot();
    }

    public void LoadFile()
    {
        population.LoadFile();
    }

    public void SaveFile()
    {
        population.SaveFile();
    }
}
