using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject[] levelPrefabs;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private CoinManager coinManager;
    [SerializeField] private ParticleSystem _finishParticles1;
    [SerializeField] private ParticleSystem _finishParticles2;
    [SerializeField] private ParticleSystem _finishParticles3;

    public event Action<int> OnLevelCompleted;

    internal Canvas winCanvas;
    private GameObject currentLevelInstance;
    private GameObject currentPlayer;
    private int currentLevelIndex = -1;

    public int CurrentLevelIndex => currentLevelIndex;
    public bool IsLastLevel => currentLevelIndex >= levelPrefabs.Length - 1;

    private void Start()
    {
        InitializeWinCanvas();
        ConfigureParticles();
    }

    private void InitializeWinCanvas()
    {
        GameObject winCanvasObj = GameObject.FindGameObjectWithTag("WinCanvas");
        if (winCanvasObj != null)
        {
            winCanvas = winCanvasObj.GetComponent<Canvas>();
        }
    }

    public void LoadLevel(int levelIndex)
    {
        CleanUpCurrentLevel();

        if (levelIndex >= 0 && levelIndex < levelPrefabs.Length)
        {
            currentLevelIndex = levelIndex;
            currentLevelInstance = Instantiate(levelPrefabs[levelIndex]);
            SpawnPlayer();
            HideWinScreen();
        }
        else if (levelIndex >= levelPrefabs.Length)
        {
            ShowFinalWinScreen();
        }
    }

    public void CleanUpCurrentLevel()
    {
        if (currentLevelInstance != null) Destroy(currentLevelInstance);
        if (currentPlayer != null) Destroy(currentPlayer);
    }

    private void SpawnPlayer()
    {
        Transform spawnPoint = FindSpawnPoint();
        if (spawnPoint != null)
        {
            currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    private Transform FindSpawnPoint()
    {
        Transform spawnPoint = currentLevelInstance.transform.Find("PlayerSpawn");
        if (spawnPoint == null)
        {
            foreach (Transform child in currentLevelInstance.transform)
            {
                if (child.CompareTag("PlayerSpawn"))
                {
                    return child;
                }
            }
        }
        return spawnPoint;
    }

    public void ShowWinScreen()
    {
        if (winCanvas != null)
        {
            winCanvas.enabled = true;
            int coinsEarned = 10 * (currentLevelIndex + 1);
            coinManager?.ShowEarnedCoins(coinsEarned);
            coinManager?.AddCoins(coinsEarned);

            var winScreen = winCanvas.GetComponent<PauseScreenController>();
            if (winScreen != null)
            {
                winScreen.SetNextButtonVisibility(!IsLastLevel);
            }
        }
        timeManager?.PauseGame();
        OnLevelCompleted?.Invoke(currentLevelIndex);
    }

    public void ShowFinalWinScreen()
    {
        if (winCanvas != null)
        {
            winCanvas.enabled = true;
            int coinsEarned = 10 * levelPrefabs.Length;
            coinManager?.ShowEarnedCoins(coinsEarned);
            coinManager?.AddCoins(coinsEarned);

            var winScreen = winCanvas.GetComponent<PauseScreenController>();
            if (winScreen != null)
            {
                winScreen.SetNextButtonVisibility(false);
            }
        }
        timeManager?.PauseGame();
        OnLevelCompleted?.Invoke(currentLevelIndex);
    }

    public void HideWinScreen()
    {
        var winScreen = winCanvas.GetComponent<PauseScreenController>();
        winCanvas.enabled = false;
        winScreen?.Close();
        timeManager?.ResumeGame();
    }

    private void ConfigureParticles()
    {
        SetUnscaledTime(_finishParticles1);
        SetUnscaledTime(_finishParticles2);
        SetUnscaledTime(_finishParticles3);
    }

    private void SetUnscaledTime(ParticleSystem ps)
    {
        if (ps == null) return;

        var main = ps.main;
        main.useUnscaledTime = true;

        ps.gameObject.SetActive(true);
        ps.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void PlayFinishParticles()
    {
        PlayParticleSystem(_finishParticles1);
        PlayParticleSystem(_finishParticles2);
        PlayParticleSystem(_finishParticles3);
    }

    private void PlayParticleSystem(ParticleSystem ps)
    {
        if (ps == null) return;

        ps.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play();
    }

    public void ResetLevel() => LoadLevel(currentLevelIndex);
    public void LoadNextLevel() => LoadLevel(currentLevelIndex + 1);
}