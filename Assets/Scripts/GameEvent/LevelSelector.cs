using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    private int selectedLevelIndex;

    public void SetSelectedLevel(int levelIndex) => selectedLevelIndex = levelIndex;
    public int GetSelectedLevel() => selectedLevelIndex;
}