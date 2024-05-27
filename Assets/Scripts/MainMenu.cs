using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Volume slider
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private GameObject scoreBoard;
    [SerializeField] private GameObject scoreEntryPrefab;

    private void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
        LoadScores();
    }
    
    void LoadScores()
    {
        scoreBoard.SetActive(true);
        // Read from PlayerPrefs
        // Create a text entry for each playtime-timestamp pair and parent it to the score board   
        for (int i = 0; i < 10; i++)
        {
            string scoreEntry = PlayerPrefs.GetString("BestTime" + i);
            var parts = scoreEntry.Split(' ');
            if (parts.Length < 2)
            {
                break;
            }
            string playTime = parts[0];
            string timestamp = parts[1];
            if (scoreEntry != "")
            {
                GameObject entry = Instantiate(scoreEntryPrefab, scoreBoard.transform);
                entry.GetComponent<TMPro.TextMeshProUGUI>().text = "Playtime: " + playTime + " Timestamp: " + timestamp;
            }
            else
            {
                break;
            }
        }
    }
    
    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("Volume", volume);
        AudioListener.volume = volume;
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        SetVolume(1f);
        Application.Quit();
    }
}
