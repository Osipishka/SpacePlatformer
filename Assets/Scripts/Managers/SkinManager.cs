using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    [System.Serializable]
    public class Skin
    {
        public Sprite sprite;
        public int price;
        [HideInInspector] public bool purchased;
        [HideInInspector] public bool equipped;
    }

    [Header("UI Elements")]
    [SerializeField] private Image skinDisplay;
    [SerializeField] private GameObject priceContainer;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button equipButton;

    [Header("Game Objects")]
    [SerializeField] private Transform playerTransform;
    private SpriteRenderer playerSpriteRenderer;

    [Header("Skin Data")]
    [SerializeField] private List<Skin> skins = new List<Skin>();

    [Header("Sound")]
    [SerializeField] private SoundManager soundManager;

    private CoinManager coinManager;
    private int currentSkinIndex = 0;

    private const string PURCHASE_KEY_PREFIX = "SkinPurchased_";
    private const string EQUIP_KEY_PREFIX = "SkinEquipped_";

    private void Start()
    {
        Initialize(FindObjectOfType<CoinManager>());
    }

    public void Initialize(CoinManager coinMgr)
    {
        coinManager = coinMgr;

        if (playerTransform != null)
        {
            playerSpriteRenderer = playerTransform.GetComponentInChildren<SpriteRenderer>(true);
        }

        LoadSkinsData();

        if (skins.Count > 0 && !skins[0].purchased)
        {
            skins[0].purchased = true;
            PlayerPrefs.SetInt(PURCHASE_KEY_PREFIX + 0, 1);
        }

        ShowSkin(currentSkinIndex);
    }

    private void LoadSkinsData()
    {
        for (int i = 0; i < skins.Count; i++)
        {
            skins[i].purchased = PlayerPrefs.GetInt(PURCHASE_KEY_PREFIX + i, i == 0 ? 1 : 0) == 1;
            skins[i].equipped = PlayerPrefs.GetInt(EQUIP_KEY_PREFIX + i, 0) == 1;

            if (skins[i].equipped && playerSpriteRenderer != null)
            {
                playerSpriteRenderer.sprite = skins[i].sprite;
                currentSkinIndex = i;
            }
        }
    }

    public void ShowSkin(int index)
    {
        if (index < 0 || index >= skins.Count) return;

        currentSkinIndex = index;
        var skin = skins[index];

        skinDisplay.sprite = skin.sprite;

        priceContainer.SetActive(!skin.purchased);
        priceText.text = skin.price.ToString();

        buyButton.gameObject.SetActive(!skin.purchased);
        buyButton.interactable = !skin.purchased && (coinManager != null && coinManager.GetCoins() >= skin.price);

        equipButton.gameObject.SetActive(skin.purchased);
        equipButton.interactable = skin.purchased && !skin.equipped;
    }

    public void BuyCurrentSkin()
    {
        if (coinManager == null) return;

        if (coinManager.SpendCoins(skins[currentSkinIndex].price))
        {
            skins[currentSkinIndex].purchased = true;
            PlayerPrefs.SetInt(PURCHASE_KEY_PREFIX + currentSkinIndex, 1);
            PlayerPrefs.Save();
            ShowSkin(currentSkinIndex);
        }
    }

    public void EquipCurrentSkin()
    {
        if (!skins[currentSkinIndex].purchased) return;

        for (int i = 0; i < skins.Count; i++)
        {
            skins[i].equipped = false;
            PlayerPrefs.SetInt(EQUIP_KEY_PREFIX + i, 0);
        }

        skins[currentSkinIndex].equipped = true;
        PlayerPrefs.SetInt(EQUIP_KEY_PREFIX + currentSkinIndex, 1);
        PlayerPrefs.Save();

        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.sprite = skins[currentSkinIndex].sprite;
        }
        soundManager?.PlaySkinSelect();
        ShowSkin(currentSkinIndex);
    }

    public void RefreshSkinButtons() => ShowSkin(currentSkinIndex);

    public void NextSkin()
    {
        currentSkinIndex = (currentSkinIndex + 1) % skins.Count;
        ShowSkin(currentSkinIndex);
    }

    public void PreviousSkin()
    {
        currentSkinIndex = (currentSkinIndex - 1 + skins.Count) % skins.Count;
        ShowSkin(currentSkinIndex);
    }
}