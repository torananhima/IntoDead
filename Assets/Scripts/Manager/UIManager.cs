using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player")]
    [Header("Status")]
    [SerializeField] Image hpBar_image;
    [SerializeField] Image hpBarFollow_image;
    [SerializeField] float barLerpSpeed = 3f;
    [SerializeField] Image staminaBar_image;
    [Header("Inventory")]
    [SerializeField] Image[] inventorySlots_image;
    [SerializeField] Image slotIndicator;

    [SerializeField] TMP_Text pistolAmmo_text;
    [SerializeField] TMP_Text rifleAmmo_text;
    [SerializeField] TMP_Text sniperAmmo_text;
    [SerializeField] WeaponAmmoUI weaponAmmoUI;

    [Header("Indicator")]
    [SerializeField] InteractIndicator interactIndicator;

    private float halfWidthScreen;
    private float halfHeightScreen;
    public static UIManager instance;
    Player player;
    PlayerInventory playerInventory;
    Camera mainCam;

    Coroutine HpBarLerp;

    Color hasItemColor = new Color(1,1,1,1);
    Color noItemColor = new Color(1,1,1, 0);
    public bool IsAboveHalfScreen(Vector2 screenPos)
    {
        return screenPos.y >= halfHeightScreen;
    }
    public bool IsRightHalfScreen(Vector2 screenPos)
    {
        return screenPos.x >= halfWidthScreen;
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        mainCam = Camera.main;
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        playerInventory = player.GetComponent<PlayerInventory>();

        playerInventory.OnInventoryChanged += ChangeInventorySlot;
;

        player._interact.OnInteractableInRange += ShowInteractIndicator;
        player._interact.OnNoInteractableInRange += HideInteractIndicator;
        player._itemHandler.OnWeaponAmmoChange += UpdateWeaponAmmo;

        halfWidthScreen = Screen.width / 2;
        halfHeightScreen = Screen.height / 2;

        StartCoroutine(DelayInitializeUI());
        HideInteractIndicator();
    }

    IEnumerator DelayInitializeUI()
    {
        yield return null;

        Canvas.ForceUpdateCanvases();
        ChangeInventorySlot();

    }
    private void OnDisable()
    {
        playerInventory.OnInventoryChanged -= ChangeInventorySlot;
        player._interact.OnInteractableInRange -= ShowInteractIndicator;
        player._interact.OnNoInteractableInRange -= HideInteractIndicator;
        player._itemHandler.OnWeaponAmmoChange -= UpdateWeaponAmmo;
    }
    public void ChangeInventorySlot()
    {
        List<ItemInstance> items = new(playerInventory.Items);
        for (int i = 0; i < inventorySlots_image.Length; i++)
        {
            if (i < items.Count)
            {
                if (items[i] == null || items[i].itemData == null)
                {
                    //inventorySlots_image[i].gameObject.SetActive(false);
                    //inventorySlots_image[i].sprite = null;
                    inventorySlots_image[i].color = noItemColor;
                    continue;
                }
                //inventorySlots_image[i].gameObject.SetActive(true);
                inventorySlots_image[i].color = hasItemColor;
                inventorySlots_image[i].sprite = items[i].itemData.icon;
            }
            else
            {
                //inventorySlots_image[i].gameObject.SetActive(false);
                inventorySlots_image[i].color = noItemColor;
                inventorySlots_image[i].sprite = null;
            }
            if (i >= inventorySlots_image.Length)
            {
                Debug.LogWarning("Index out of Image range");
                break;
            }

        }
        ChangeSelectingSlot();
        UpdateAmmo(new AmmoContext(playerInventory.pistolAmmo, playerInventory.rifleAmmo, playerInventory.sniperAmmo));
    }

    public void UpdateAmmo(AmmoContext ammoContext)
    {
        pistolAmmo_text.text = $"P {ammoContext.pistolAmmo}";
        rifleAmmo_text.text = $"R {ammoContext.rifleAmmo}";
        sniperAmmo_text.text = $"S {ammoContext.sniperAmmo}";
    }
    public void ChangeSelectingSlot()
    {
        slotIndicator.transform.SetParent(inventorySlots_image[playerInventory.SelectedSlot].transform);
        slotIndicator.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        ItemInstance holdItem = playerInventory.GetSelectedItem();
        if (holdItem != null)
        {
            if (holdItem.itemData is RangedWeaponData gun)
            {
                ShowWeaponAmmo();
                UpdateWeaponAmmo(holdItem.CurrentAmmo, holdItem.MaxMagazine);
            }
            else
            {
                HideeWeaponAmmo();
            }
        }
        //RectTransform indicator = slotIndicator.GetComponent<RectTransform>();
        //RectTransform slot = inventorySlots_image[playerInventory.SelectedSlot].GetComponent<RectTransform>();

        //indicator.position = slot.position;
    }

    public void UpdateHpBar(float currentHp, float maxHp)
    {
        hpBar_image.fillAmount = currentHp / maxHp;
        if(HpBarLerp != null)
        {
            StopCoroutine(HpBarLerp);
            HpBarLerp = null;
        }

        HpBarLerp = StartCoroutine(FollowHpBar(currentHp, maxHp));
    }

    IEnumerator FollowHpBar(float currentHp, float maxHp)
    {
        float fillAmount = currentHp / maxHp;
        while (hpBarFollow_image.fillAmount != fillAmount)
        {
            hpBarFollow_image.fillAmount = Mathf.Lerp(hpBarFollow_image.fillAmount, fillAmount, Time.deltaTime * barLerpSpeed);
            yield return null;
        }
        StopCoroutine(HpBarLerp);
        HpBarLerp = null;
    }
    public void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        staminaBar_image.fillAmount = currentStamina / maxStamina;
    }

    public void ShowWeaponAmmo()
    {
        weaponAmmoUI.gameObject.SetActive(true);
    }
    public void UpdateWeaponAmmo(int currentAmmo, int maxMagazine)
    {
        weaponAmmoUI.UpdateAmmoUI(currentAmmo, maxMagazine);
    }
    public void HideeWeaponAmmo()
    {
        weaponAmmoUI.gameObject.SetActive(false);
    }
    public void ShowInteractIndicator(Transform interactObject, string actionName)
    {
        Vector2 screenPos = mainCam.WorldToScreenPoint(interactObject.position);
        interactIndicator.Show();
        interactIndicator.UpdateIndicator(screenPos, actionName);
    }

    public void HideInteractIndicator()
    {
        interactIndicator.Hide();
    }
}
