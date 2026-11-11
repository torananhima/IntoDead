using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public enum ItemType
    {
        MeleeWeapon, RangedWeapon, HealingItem, ThrowableItem

    }
    [SerializeField]private string itemId;
    [SerializeField]private string displayName;
    public Sprite icon;
    public GameObject worldPrefab;
    [SerializeField]private ItemType itemType;
    [SerializeField]private bool isStackable;
    public int maxStackSize;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public bool IsStackable => isStackable;
    public int MaxStackSize => isStackable
        ? Mathf.Max(1, maxStackSize)
        : 1;

    public bool CanStackWith(ItemData other)
    {
        if(other == null) return false;
        return isStackable && itemId == other.itemId;
    }

    public  void CreateInstance(int amount)
    {
        ItemInstance instance = new ItemInstance(this, amount);
    }
}
