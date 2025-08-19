using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    [Header("Настройки")]
    public string managerTag = "SoundManager";

    [Header("Аудио источники")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Звуки игры")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip skinSelectSound;

    private const string MUSIC_PREF_KEY = "MusicEnabled";
    private const string SOUNDS_PREF_KEY = "SoundsEnabled";

    private static SoundManager instance;

    private List<Button> musicOnButtons = new List<Button>();
    private List<Button> musicOffButtons = new List<Button>();
    private List<Button> soundsOnButtons = new List<Button>();
    private List<Button> soundsOffButtons = new List<Button>();

    public static SoundManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (!gameObject.CompareTag(managerTag))
            {
                gameObject.tag = managerTag;
            }

            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        bool musicEnabled = IsMusicEnabled();
        bool soundsEnabled = IsSoundsEnabled();

        SetMusicActive(musicEnabled);
        SetSoundsActive(soundsEnabled);
    }
    public void ToggleMusic()
    {
        bool newState = !IsMusicEnabled();
        SetMusicActive(newState);
        PlayButtonClick();
        UpdateAllMusicButtons();
    }

    public void SetMusicActive(bool active)
    {
        musicSource.mute = !active;
        PlayerPrefs.SetInt(MUSIC_PREF_KEY, active ? 1 : 0);
        PlayerPrefs.Save();
        UpdateAllMusicButtons();
    }

    public bool IsMusicEnabled() => PlayerPrefs.GetInt(MUSIC_PREF_KEY, 1) == 1;

    public void ToggleSounds()
    {
        bool newState = !IsSoundsEnabled();
        SetSoundsActive(newState);
        PlayButtonClick();
        UpdateAllSoundButtons();
    }

    public void SetSoundsActive(bool active)
    {
        sfxSource.mute = !active;
        PlayerPrefs.SetInt(SOUNDS_PREF_KEY, active ? 1 : 0);
        PlayerPrefs.Save();
        UpdateAllSoundButtons();
    }

    public bool IsSoundsEnabled() => PlayerPrefs.GetInt(SOUNDS_PREF_KEY, 1) == 1;

    public void RegisterMusicButtons(Button musicOnButton, Button musicOffButton)
    {
        if (musicOnButton != null && !musicOnButtons.Contains(musicOnButton))
            musicOnButtons.Add(musicOnButton);

        if (musicOffButton != null && !musicOffButtons.Contains(musicOffButton))
            musicOffButtons.Add(musicOffButton);

        UpdateMusicButtons(musicOnButton, musicOffButton);
    }

    public void RegisterSoundButtons(Button soundsOnButton, Button soundsOffButton)
    {
        if (soundsOnButton != null && !soundsOnButtons.Contains(soundsOnButton))
            soundsOnButtons.Add(soundsOnButton);

        if (soundsOffButton != null && !soundsOffButtons.Contains(soundsOffButton))
            soundsOffButtons.Add(soundsOffButton);

        UpdateSoundButtons(soundsOnButton, soundsOffButton);
    }

    public void UnregisterMusicButtons(Button musicOnButton, Button musicOffButton)
    {
        if (musicOnButton != null) musicOnButtons.Remove(musicOnButton);
        if (musicOffButton != null) musicOffButtons.Remove(musicOffButton);
    }

    public void UnregisterSoundButtons(Button soundsOnButton, Button soundsOffButton)
    {
        if (soundsOnButton != null) soundsOnButtons.Remove(soundsOnButton);
        if (soundsOffButton != null) soundsOffButtons.Remove(soundsOffButton);
    }

    private void UpdateAllMusicButtons()
    {
        bool musicEnabled = IsMusicEnabled();

        foreach (var button in musicOnButtons)
            if (button != null) button.gameObject.SetActive(musicEnabled);

        foreach (var button in musicOffButtons)
            if (button != null) button.gameObject.SetActive(!musicEnabled);
    }

    private void UpdateAllSoundButtons()
    {
        bool soundsEnabled = IsSoundsEnabled();

        foreach (var button in soundsOnButtons)
            if (button != null) button.gameObject.SetActive(soundsEnabled);

        foreach (var button in soundsOffButtons)
            if (button != null) button.gameObject.SetActive(!soundsEnabled);
    }

    private void UpdateMusicButtons(Button onButton, Button offButton)
    {
        if (onButton == null || offButton == null) return;

        bool musicEnabled = IsMusicEnabled();
        onButton.gameObject.SetActive(musicEnabled);
        offButton.gameObject.SetActive(!musicEnabled);
    }

    private void UpdateSoundButtons(Button onButton, Button offButton)
    {
        if (onButton == null || offButton == null) return;

        bool soundsEnabled = IsSoundsEnabled();
        onButton.gameObject.SetActive(soundsEnabled);
        offButton.gameObject.SetActive(!soundsEnabled);
    }

    public void PlayJump() => PlaySFX(jumpSound);
    public void PlayWin() => PlaySFX(winSound);
    public void PlayLose() => PlaySFX(loseSound);
    public void PlayButtonClick() => PlaySFX(buttonClickSound);
    public void PlaySkinSelect() => PlaySFX(skinSelectSound);

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null && !sfxSource.mute)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}