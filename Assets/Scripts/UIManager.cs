using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] Image[] inventorySlots_image;
    [SerializeField] Image hpBar_image;
    [SerializeField] Image slotIndicator;

    public static UIManager instance;
    Player player;
    PlayerInventory playerInventory;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        playerInventory = player.GetComponent<PlayerInventory>();

        playerInventory.OnInventoryChanged += ChangeInventorySlot;
        ChangeInventorySlot();
        slotIndicator.transform.position = inventorySlots_image[0].transform.position;
    }
    private void OnDisable()
    {
        playerInventory.OnInventoryChanged -= ChangeInventorySlot;
    }
    public void ChangeInventorySlot()
    {
        List<ItemInstance> items = playerInventory.items;
        for (int i = 0; i < items.Count; i++)
        {
            inventorySlots_image[i].sprite = items[i].itemData.icon;
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
        slotIndicator.transform.position = inventorySlots_image[player._inventory.selectedSlot].transform.position;
    }

    public void UpdateHpBar(int currentHp, int maxHp)
    {
        hpBar_image.fillAmount = (float)currentHp / (float)maxHp;
    }
}
