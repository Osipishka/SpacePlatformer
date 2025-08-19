using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public void PauseGame()
    {
        if (!IsPaused)
            Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (IsPaused)
            Time.timeScale = 1f;
    }

    public bool IsPaused => Time.timeScale == 0f;
}