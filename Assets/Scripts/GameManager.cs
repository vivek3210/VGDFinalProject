using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Lives")]
    public int startingLives = 3;
    public int currentLives;

    [Header("UI")]
    public GameObject wastedPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI livesText;

    [Header("Player")]
    public Transform player;
    public Transform respawnPoint;

    private bool isProcessingCaught = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentLives = startingLives;
    }

    public void PlayerCaught()
    {
        if (isProcessingCaught) return;
        StartCoroutine(HandlePlayerCaught());
    }

    IEnumerator HandlePlayerCaught()
    {
        isProcessingCaught = true;
        currentLives--;

        if (wastedPanel != null)
        {
            wastedPanel.SetActive(true);
        }

        // if (livesText != null)
        // {
        //     livesText.text = "You have " + currentLives + " lives remaining";
        // }

        yield return new WaitForSeconds(2f);

        if (wastedPanel != null)
        {
            wastedPanel.SetActive(false);
        }

        if (currentLives > 0)
        {
            RespawnPlayer();
            isProcessingCaught = false;
        }
        else
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            Time.timeScale = 0f;
        }
    }

    void RespawnPlayer()
    {
        if (player != null && respawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();

            if (cc != null)
            {
                cc.enabled = false;
                player.position = respawnPoint.position;
                player.rotation = respawnPoint.rotation;
                cc.enabled = true;
            }
            else
            {
                player.position = respawnPoint.position;
                player.rotation = respawnPoint.rotation;
            }
        }
    }
    public GameObject winPanel;

    public void PlayerWon()
    {
        Time.timeScale = 0f; // pause game

        if (winPanel != null)
            winPanel.SetActive(true);
    }
    public void PlayAgain()
    {
        Time.timeScale = 1f; // IMPORTANT (unpause)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void TryAgain()
    {
        if (wastedPanel != null)
            wastedPanel.SetActive(false);

        RespawnPlayer();
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}