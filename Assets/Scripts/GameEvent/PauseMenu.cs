using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private TimeManager timeManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (timeManager.IsPaused)
        {
            pauseCanvas.SetActive(false);
            timeManager.ResumeGame();
        }
        else
        {
            pauseCanvas.SetActive(true);
            timeManager.PauseGame();
        }
    }
}