using UnityEngine;

public class DeathBorder : MonoBehaviour
{
    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private SoundManager soundManager;
    private PauseScreenController loseScreen;
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        if (loseCanvas != null)
        {
            soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
            loseScreen = loseCanvas.GetComponent<PauseScreenController>();
            loseScreen?.Close();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            soundManager?.PlayLose();
            loseScreen?.Open();
        }
    }

    public void RestartLevel()
    {
        loseScreen?.Close();
        levelManager?.LoadLevel(levelManager.CurrentLevelIndex);
    }

    public void DisableLoseCanvas()
    {
        loseCanvas.GetComponent<Canvas>().enabled = false;
        loseScreen?.Close();
    }
}