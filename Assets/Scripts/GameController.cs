using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    [SerializeField] private GameObject winScreen;

    private float timeElapsed = 0f;
    public GameObject player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            player = GameObject.FindGameObjectWithTag("Player");
            SceneManager.sceneLoaded += OnSceneLoaded;
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

    // When scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.layer = LayerMask.NameToLayer("Default");
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
            winScreen.SetActive(true);
            winScreen.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                "You Win!\nTotal time: " + timeElapsed.ToString("F1") + " seconds";
            player.GetComponent<LaserShooter>().enabled = false;
        }
    }

    public void SaveTime()
    {
        // Save the time elapsed to PlayerPrefs along with the timestamp
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var bestTimes = GetBestTimes();
        bestTimes.Add(timeElapsed, timestamp);
        bestTimes = bestTimes.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        for (int i = 0; i < 10; i++)
        {
            if (i < bestTimes.Count)
            {
                var bestTime = bestTimes.ElementAt(i);
                PlayerPrefs.SetString("BestTime" + i, bestTime.Key + " " + bestTime.Value);
            }
            else
            {
                PlayerPrefs.SetString("BestTime" + i, "");
            }
        }
    }

    public Dictionary<float, string> GetBestTimes()
    {
        Dictionary<float, string> bestTimes = new Dictionary<float, string>();
        for (int i = 0; i < 10; i++)
        {
            var bestTimeString = PlayerPrefs.GetString("BestTime" + i);
            if (bestTimeString != "")
            {
                var components = bestTimeString.Split(" ");
                var bestTime = float.Parse(components[0]);
                var timestamp = components[1];
                bestTimes.Add(bestTime, timestamp);
            }
            else
            {
                break;
            }
        }

        return bestTimes;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}