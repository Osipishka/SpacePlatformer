using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreenController : UIAnimationController
{
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private Button nextButton;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();

        if (timeManager == null)
        {
            timeManager = FindObjectOfType<TimeManager>();
        }

        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    public override void Open()
    {
        if (isOpen) return;

        base.Open();

        if (timeManager != null && !timeManager.IsPaused)
        {
            timeManager.PauseGame();
        }
    }

    public void Close(System.Action onComplete = null)
    {
        if (!isOpen) return;

        if (timeManager != null && timeManager.IsPaused)
        {
            timeManager.ResumeGame();
        }

        if (animator != null)
        {
            animator.SetTrigger("Close");
            StartCoroutine(WaitForAnimationComplete(() =>
            {
                isOpen = false;
                onComplete?.Invoke();
            }));
        }
        else
        {
            isOpen = false;
            onComplete?.Invoke();
        }
    }

    public void SetNextButtonVisibility(bool visible)
    {
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(visible);
        }
    }

    public void OnNextButtonClicked()
    {
        var levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.LoadNextLevel();
        }
        Close();
    }

    public void OnRestartButtonClicked()
    {
        var levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.ResetLevel();
        }
        Close();
    }

    public void ResetAnimationState()
    {
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
        isOpen = false;
    }

    public void OnMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
    private System.Collections.IEnumerator WaitForAnimationComplete(System.Action callback)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        callback?.Invoke();
    }
}