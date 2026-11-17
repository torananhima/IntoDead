using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text amountText;
    public Image ItemIcon => itemIcon;

    private Color hasItemColor = Color.white;
    private Color noItemColor = new Color(1, 1, 1, 0);

    public void UpdateSlot(InventoryContext context)
    {
        if (!context.HasItem)
        {
            ClearSlot();
            return;
        }

        itemIcon.sprite = context.itemData.icon;
        itemIcon.color = hasItemColor;

        amountText.text = context.amount > 1
            ? context.amount.ToString()
            : string.Empty;
    }

    public void ClearSlot()
    {
        itemIcon.sprite = null;
        itemIcon.color = noItemColor;
        amountText.text = string.Empty;
    }
}
