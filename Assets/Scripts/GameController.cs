using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    
    private  float timeElapsed = 0f;
    private GameObject player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        timeText.text = "Time: " + timeElapsed.ToString("F1");
    }
    
    public void LoseGame()
    {
        Debug.Log("Game Over!");
        // Reload the level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void WinGame()
    {
        Debug.Log("You Win this level!");
        // Load the next level if it is not the last level
        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Debug.Log("You have completed all levels!");
        }
        
        // If it is the last level, save the time elapsed to PlayerPrefs
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            SaveTime();
        }
    }
    
    public void SaveTime()
    {
        // Save the time elapsed to PlayerPrefs
        float[] bestTimes = new float[10];
        for (int i = 0; i < 10; i++)
        {
            bestTimes[i] = PlayerPrefs.GetFloat("BestTime" + i, 0);
        }
        
        // Check if the current time is one of the best times
        for (int i = 0; i < 10; i++)
        {
            if (timeElapsed < bestTimes[i])
            {
                // Shift the other times
                for (int j = 9; j > i; j--)
                {
                    bestTimes[j] = bestTimes[j - 1];
                }
                bestTimes[i] = timeElapsed;
                break;
            }
        }
        
        // Save the best times to PlayerPrefs
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetFloat("BestTime" + i, bestTimes[i]);
        }
    }
    
    public Dictionary<int, float> GetBestTimes()
    {
        Dictionary<int, float> bestTimes = new Dictionary<int, float>();
        for (int i = 0; i < 10; i++)
        {
            bestTimes.Add(i, PlayerPrefs.GetFloat("BestTime" + i, 0));
        }
        return bestTimes;
    } 
    
}
