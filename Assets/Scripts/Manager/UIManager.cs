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
    [SerializeField] Image movementState_image;
    [Header("Inventory")]
    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] Image slotIndicator;

    [SerializeField] TMP_Text pistolAmmo_text;
    [SerializeField] TMP_Text rifleAmmo_text;
    [SerializeField] TMP_Text sniperAmmo_text;
    [SerializeField] WeaponAmmoUI weaponAmmoUI;

    [Header("Indicator")]
    [SerializeField] InteractIndicator interactIndicator;
    [SerializeField] ItemUsedIndicator itemUsedIndicator;

    private float halfWidthScreen;
    private float halfHeightScreen;
    public static UIManager instance;
    Player player;
    PlayerInventory playerInventory;
    Camera mainCam;

    Coroutine HpBarLerp;
    Coroutine WeaponAmmoLerp;

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

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i >= items.Count || items[i] == null || items[i].itemData == null)
            {
                inventorySlots[i].ClearSlot();
                continue;
            }

            ItemInstance item = items[i];

            InventoryContext context = new InventoryContext(
                item.itemData,
                item.Amount
            );

            inventorySlots[i].UpdateSlot(context);
        }

        ChangeSelectingSlot();

        UpdateAmmo(new AmmoContext(
            playerInventory.pistolAmmo,
            playerInventory.rifleAmmo,
            playerInventory.sniperAmmo
        ));
    }

    public void UpdateAmmo(AmmoContext ammoContext)
    {
        pistolAmmo_text.text = $"{ammoContext.pistolAmmo}";
        rifleAmmo_text.text = $"{ammoContext.rifleAmmo}";
        sniperAmmo_text.text = $"{ammoContext.sniperAmmo}";
    }

    public void ShowItemDelay(float delay)
    {
        itemUsedIndicator.Initialize(delay);
    }
    public void HideItemDelay()
    {
        itemUsedIndicator.Hide();
    }
    public void ChangeSelectingSlot()
    {
        slotIndicator.transform.SetParent(
                inventorySlots[playerInventory.SelectedSlot].ItemIcon.transform
            );
        slotIndicator.transform.SetAsFirstSibling();

        slotIndicator.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        ItemInstance holdItem = playerInventory.GetSelectedItem();

        if (holdItem != null && holdItem.itemData is RangedWeaponData)
        {
            ShowWeaponAmmo();
            UpdateWeaponAmmo(holdItem.CurrentAmmo, holdItem.MaxMagazine);
        }
        else
        {
            HideeWeaponAmmo();
        }
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
        if (HpBarLerp != null)
        {
            StopCoroutine(HpBarLerp);
            HpBarLerp = null;
        }
    }
    public void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        staminaBar_image.fillAmount = currentStamina / maxStamina;
    }

    public void ShowWeaponAmmo()
    {
        weaponAmmoUI.gameObject.SetActive(true);

        if (WeaponAmmoLerp != null)
        {
            StopCoroutine(WeaponAmmoLerp);
            WeaponAmmoLerp = null;
        }
        WeaponAmmoLerp = StartCoroutine(UpdateWeaponAmmo());
    }
    public void UpdateWeaponAmmo(int currentAmmo, int maxMagazine)
    {
        weaponAmmoUI.UpdateAmmoUI(currentAmmo, maxMagazine);
    }
    public void HideeWeaponAmmo()
    {
        weaponAmmoUI.gameObject.SetActive(false);
        if (WeaponAmmoLerp != null)
        {
            StopCoroutine(WeaponAmmoLerp);
            WeaponAmmoLerp = null;
        }
    }

    IEnumerator UpdateWeaponAmmo()
    {
        while (true)
        { 
            weaponAmmoUI.SetTargetPosition(!IsRightHalfScreen(player._input.MousePos));
            yield return null;
        }
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

    public void ChangeMovementState(Sprite newSprite)
    {
        if (newSprite == null) return;
        if(movementState_image != null)
        {
            movementState_image.sprite = newSprite;
        }
    }
}
