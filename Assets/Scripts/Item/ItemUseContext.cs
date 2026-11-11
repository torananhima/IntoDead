using UnityEngine;

public class ItemUseContext
{
    public ItemUseHandler itemUseHandler;
    public ItemInstance itemInstance;

    public ItemUseContext(ItemUseHandler user, ItemInstance itemInstance)
    {
        this.itemUseHandler = user;
        this.itemInstance = itemInstance;
    }
}
