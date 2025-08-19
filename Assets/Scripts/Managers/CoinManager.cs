using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text mainCoinsText;
    [SerializeField]  private TMP_Text winScreenCoinsText; 

    [Header("Dependencies")]
    [SerializeField] private SkinManager skinManager;

    private int coins;

    private void Awake()
    {
        coins = PlayerPrefs.GetInt("PlayerCoins", 0);
        UpdateUI();

        GameObject winCoinsObj = GameObject.FindWithTag("WinScreenCoins");
        if (winCoinsObj != null)
        {
            winScreenCoinsText = winCoinsObj.GetComponent<TMP_Text>();
        }
    }

    public void ShowEarnedCoins(int amount)
    {
        if (winScreenCoinsText != null)
        {
            winScreenCoinsText.text = "+" + amount;
        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt("PlayerCoins", coins);
        PlayerPrefs.Save();
        UpdateUI();
        skinManager?.RefreshSkinButtons();
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            PlayerPrefs.SetInt("PlayerCoins", coins);
            PlayerPrefs.Save();
            UpdateUI();
            skinManager?.RefreshSkinButtons();
            return true;
        }
        return false;
    }

    public int GetCoins() => coins;

    private void UpdateUI()
    {
        if (mainCoinsText != null)
            mainCoinsText.text = coins.ToString();
    }

    public void SetSkinManager(SkinManager manager)
    {
        skinManager = manager;
    }
}