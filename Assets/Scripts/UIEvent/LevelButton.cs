using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private int levelIndex;
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            GetComponent<Button>().onClick.AddListener(() => levelManager.LoadLevel(levelIndex));
        }
    }
}