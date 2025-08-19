using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;

    [System.Serializable]
    public class LevelInfo
    {
        public Sprite unlockedSprite;
        public Sprite lockedSprite;
        public Button levelButton;
    }

    [Header("Level Settings")]
    [SerializeField] private List<LevelInfo> allLevels = new List<LevelInfo>();
    [SerializeField] private int levelsPerPage = 9;

    [Header("Page Settings")]
    [SerializeField] private GameObject[] pageContainers;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;

    private int currentPage = 0;
    private int totalPages;

    private void Start()
    {
        totalPages = Mathf.CeilToInt((float)allLevels.Count / levelsPerPage);
        SetupPages();
        UpdateNavigationButtons();
        LoadLevelProgress();

        levelManager.OnLevelCompleted += OnLevelComplete;
    }

    private void OnDestroy()
    {
        if (levelManager != null)
        {
            levelManager.OnLevelCompleted -= OnLevelComplete;
        }
    }

    private void SetupPages()
    {
        int levelIndex = 0;

        for (int page = 0; page < pageContainers.Length; page++)
        {
            var pageContainer = pageContainers[page];
            pageContainer.SetActive(page == 0);

            var levelButtons = pageContainer.GetComponentsInChildren<Button>(true);

            for (int i = 0; i < levelsPerPage; i++)
            {
                if (levelIndex >= allLevels.Count) break;

                int currentLevel = levelIndex;
                levelButtons[i].onClick.RemoveAllListeners();
                levelButtons[i].onClick.AddListener(() => LoadLevel(currentLevel));

                levelIndex++;
            }
        }
    }

    private void LoadLevelProgress()
    {
        int highestCompleted = PlayerPrefs.GetInt("HighestCompletedLevel", -1);

        for (int i = 0; i < allLevels.Count; i++)
        {
            bool isUnlocked = i <= highestCompleted + 1;
            allLevels[i].levelButton.interactable = isUnlocked;
            allLevels[i].levelButton.image.sprite = isUnlocked ? allLevels[i].unlockedSprite : allLevels[i].lockedSprite;
        }
    }

    public void NextPage()
    {
        if (currentPage < totalPages - 1)
        {
            pageContainers[currentPage].SetActive(false);
            currentPage++;
            pageContainers[currentPage].SetActive(true);
            UpdateNavigationButtons();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            pageContainers[currentPage].SetActive(false);
            currentPage--;
            pageContainers[currentPage].SetActive(true);
            UpdateNavigationButtons();
        }
    }

    private void UpdateNavigationButtons()
    {
        prevPageButton.interactable = currentPage > 0;
        nextPageButton.interactable = currentPage < totalPages - 1;
    }

    private void LoadLevel(int levelIndex)
    {
        levelManager.LoadLevel(levelIndex);
    }

    public void OnLevelComplete(int levelIndex)
    {
        if (levelIndex > PlayerPrefs.GetInt("HighestCompletedLevel", -1))
        {
            PlayerPrefs.SetInt("HighestCompletedLevel", levelIndex);
            PlayerPrefs.Save();
            LoadLevelProgress();

            if (levelIndex + 1 < allLevels.Count)
            {
                allLevels[levelIndex + 1].levelButton.interactable = true;
                allLevels[levelIndex + 1].levelButton.image.sprite = allLevels[levelIndex + 1].unlockedSprite;
            }
        }
    }
}