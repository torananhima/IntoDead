using Cysharp.Threading.Tasks;
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
    [SerializeField] Image staminaBar_image;
    [Header("Inventory")]
    [SerializeField] Image[] inventorySlots_image;
    [SerializeField] Image slotIndicator;

    [Header("Indicator")]
    [SerializeField] Vector2 interactIndicatorOffset;
    [SerializeField] GameObject interactIndicator;
    [SerializeField] TMP_Text interactAction_text;

    private float halfWidthScreen;
    private float halfHeightScreen;
    public static UIManager instance;
    Player player;
    PlayerInventory playerInventory;
    Camera mainCam;

    private bool IsAboveHalfScreen(Vector2 screenPos)
    {
        return screenPos.y >= halfHeightScreen;
    }
    private bool IsRightHalfScreen(Vector2 screenPos)
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
    }
    public void ChangeInventorySlot()
    {
        List<ItemInstance> items = playerInventory.items;
        for (int i = 0; i < inventorySlots_image.Length; i++)
        {
            if (i < items.Count)
            {
                inventorySlots_image[i].gameObject.SetActive(true);
                inventorySlots_image[i].sprite = items[i].itemData.icon;
            }
            else
            {
                //inventorySlots_image[i].gameObject.SetActive(false);
                inventorySlots_image[i].sprite = null;
            }
            if (i >= inventorySlots_image.Length)
            {
                Debug.LogWarning("Index out of Image range");
                break;
            }

        }
        ChangeSelectingSlot();
    }

    public void ChangeSelectingSlot()
    {

        RectTransform indicator = slotIndicator.GetComponent<RectTransform>();
        RectTransform slot = inventorySlots_image[playerInventory.selectedSlot].GetComponent<RectTransform>();

        indicator.position = slot.position;
    }

    public void UpdateHpBar(int currentHp, int maxHp)
    {
        hpBar_image.fillAmount = (float)currentHp / (float)maxHp;
    }

    public void ShowInteractIndicator(Transform interactObject, string actionName)
    {
        if(!interactIndicator.activeSelf) interactIndicator.SetActive(true);

        Vector2 screenPos = mainCam.WorldToScreenPoint(interactObject.position);
        int xFlip = IsRightHalfScreen(screenPos) ? 1 : -1;
        int yFlip = IsAboveHalfScreen(screenPos) ? 1 : -1;
        screenPos.x += interactIndicatorOffset.x * xFlip;
        screenPos.y += interactIndicatorOffset.y * yFlip;
        interactIndicator.transform.position = screenPos;

        interactAction_text.text = actionName;
    }

    public void HideInteractIndicator()
    {
        interactIndicator.SetActive(false);
    }

}
