
using System.Collections;
using UnityEngine;

public class ItemUseHandler : MonoBehaviour
{
    [SerializeField] MeleeAttackDebug meleeAttackDebug;
    public PlayerInventory inventory { get; private set; }

    private HealingItemBehavior healingBehavior;
    private MeleeWeaponBehavior meleeWeaponBehavior;
    private RangedWeaponBehavior rangedWeaponBehavior;
    private ThrowableItemBehavior throwWeaponBehavior;

    private Player player;
    private Camera mainCam;

    [Header("Weapon State")]
    public bool isOnAttackCooldown { get; private set; }
    public bool isReloading { get; private set; }

    [Header("Firing Point")]
    [SerializeField] private Transform firePoint;

    private Coroutine cooldownRoutine;
    private Coroutine reloadRoutine;
    private bool hasStarted;
    public Transform FirePoint => firePoint != null
        ? firePoint
        : transform;

    private void Awake()
    {
        healingBehavior = new HealingItemBehavior();
        meleeWeaponBehavior = new MeleeWeaponBehavior();
        rangedWeaponBehavior = new RangedWeaponBehavior();
        throwWeaponBehavior = new ThrowableItemBehavior();

        player = GetComponent<Player>();
        inventory = GetComponent<PlayerInventory>();
        mainCam = Camera.main;
    }

    private void Start()
    {
        hasStarted = true;
        SubscribeInput();
    }

    private void OnEnable()
    {
        if (hasStarted)
            SubscribeInput();
    }

    private void OnDisable()
    {
        if (player != null && player._input != null)
        {
            player._input.InputOnUse -= TryUseSelectedItem;
            player._input.InputOnReload -= TryReload;
        }

        if (cooldownRoutine != null)
            StopCoroutine(cooldownRoutine);

        if (reloadRoutine != null)
            StopCoroutine(reloadRoutine);

        cooldownRoutine = null;
        reloadRoutine = null;

        isOnAttackCooldown = false;
        isReloading = false;
    }

    private void SubscribeInput()
    {
        if (player == null || player._input == null)
            return;

        player._input.InputOnUse -= TryUseSelectedItem;
        player._input.InputOnReload -= TryReload;

        player._input.InputOnUse += TryUseSelectedItem;
        player._input.InputOnReload += TryReload;
    }
    public void TryUseSelectedItem()
    {
        if (isOnAttackCooldown || isReloading)
            return;

        ItemInstance item = inventory.GetSelectedItem();

        if (item == null || item.itemData == null)
            return;

        ItemUseContext context = new ItemUseContext(this, item);

        IItemBehavior behavior = GetBehavior(item.itemData);

        if (behavior == null || !behavior.CanUse(context))
            return;

        behavior.Use(context);

        if (item.IsEmpty)
        {
            inventory.TryRemoveItem(item);
        }
        else
        {
            inventory.NotifyInventoryChanged();
        }
    }

    public void TryReload()
    {
        if (isOnAttackCooldown || isReloading)
            return;

        ItemInstance item = inventory.GetSelectedItem();

        if (item == null || item.itemData is not RangedWeaponData weaponData)
            return;

        // No need to reload a full magazine.
        if (item.CurrentAmmo >= item.MaxMagazine)
            return;

        // Check whether reserve ammo exists.
        if (inventory.GetAmmo(weaponData.ammoType) <= 0)
            return;

        reloadRoutine = StartCoroutine(
            ReloadCoroutine(item, weaponData)
        );
    }

    private IEnumerator ReloadCoroutine(
        ItemInstance weapon,
        RangedWeaponData weaponData)
    {
        isReloading = true;

        Debug.Log("Reloading...");

        yield return new WaitForSeconds(weaponData.reloadDuration);

        // Cancel if the player changed their selected weapon.
        if (inventory.GetSelectedItem() != weapon)
        {
            Debug.Log("Reload cancelled.");
            isReloading = false;
            reloadRoutine = null;
            yield break;
        }

        int availableAmmo = inventory.GetAmmo(weaponData.ammoType);

        int loadedAmmo = weapon.Reload(availableAmmo);

        if (loadedAmmo > 0)
        {
            inventory.RemoveAmmo(weaponData.ammoType, loadedAmmo);
            inventory.NotifyInventoryChanged();

            Debug.Log($"Reloaded {loadedAmmo} rounds.");
        }

        isReloading = false;
        reloadRoutine = null;
    }

    private IItemBehavior GetBehavior(ItemData data)
    {
        if (data is HealingItemData)
            return healingBehavior;

        if (data is RangedWeaponData)
            return rangedWeaponBehavior;

        if (data is MeleeWeaponData)
            return meleeWeaponBehavior;

        if (data is ThrowableItemData)
            return throwWeaponBehavior;

        return null;
    }

    public Vector3 GetPlayerMouseToward()
    {
        Vector2 lookPos = new Vector2(player._input.MousePos.x + transform.position.x, player._input.MousePos.y + transform.position.z);
        return new Vector3(lookPos.x, transform.position.y, lookPos.y);
    }

    public void StartAttackCooldown(float attackCd)
    {
        if (cooldownRoutine != null)
            StopCoroutine(cooldownRoutine);

        cooldownRoutine = StartCoroutine(
            AttackCooldownCoroutine(attackCd)
        );
    }

    private IEnumerator AttackCooldownCoroutine(float attackCd)
    {
        isOnAttackCooldown = true;

        yield return new WaitForSeconds(attackCd);

        isOnAttackCooldown = false;
        cooldownRoutine = null;
    }
    public void UpdateMeleeDebug(Vector3 direction)
    {
        if (meleeAttackDebug != null)
            meleeAttackDebug.UpdateAttackDirection(direction);
    }
}
