using UnityEngine;

public class ItemUseContext
{
    public ItemUseHandler itemUseHandler;
    public ItemInstance itemInstance;

    public ItemUseContext(GameObject user, ItemInstance itemInstance)
    {
        this.itemUseHandler = user.GetComponent<ItemUseHandler>();
        this.itemInstance = itemInstance;
    }
}
