using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]

public abstract class ItemData : ScriptableObject
{
    public enum ItemType
    {
        MeleeWeapon, RangedWeapon, HealingItem, ThrowableItem

    }
    [SerializeField]private string itemId;
    [SerializeField]private string displayName;
    [SerializeField]private string description;
    public Sprite icon;
    public GameObject worldPrefab;
    [SerializeField]private ItemType itemType;
    [SerializeField]private bool isStackable;
    private int maxStackSize;

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
