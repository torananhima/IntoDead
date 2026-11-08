using UnityEngine;

public interface IItemBehavior 
{
    void Use(ItemUseContext context);
    bool CanUse(ItemUseContext context);

    void AltUse(ItemUseContext context);
}
