//using System.Collections.Generic;
//using UnityEngine;

//public class WorldContainer : MonoBehaviour, IInteractable
//{
//    [SerializeField] private ContainerData data;
//    [SerializeField] private Transform dropPoint;

//    private readonly List<ItemInstance> contents = new List<ItemInstance>();

//    private bool hasGeneratedLoot;
//    private bool isOpened;

//    public string InteractionPrompt => isOpened ? string.Empty : "Open";

//    public bool CanInteract(GameObject interactor)
//    {
//        return interactor != null && !isOpened;
//    }

//    public void InitializeLoot(bool random)
//    {
//        if (hasGeneratedLoot || data == null)
//        {
//            return;
//        }

//        hasGeneratedLoot = true;

//        List<ContainerData.LootEntry> loot = new List<ContainerData.LootEntry>(data.Loot);

//        if (random)
//        {
//            Shuffle(loot);
//        }

//        foreach (ContainerData.LootEntry entry in loot)
//        {
//            if (entry == null || entry.itemData == null)
//            {
//                continue;
//            }

//            GameObject itemObject = new GameObject(entry.itemData.name);
//            itemObject.transform.SetParent(transform);
//            itemObject.SetActive(false);

//            ItemInstance item = itemObject.AddComponent<ItemInstance>();
//            item.itemData = entry.itemData;
//            item.AddAmount(entry.amount);

//            contents.Add(item);
//        }
//    }

//    public void Interact(GameObject interactor)
//    {
//        if (!CanInteract(interactor))
//        {
//            return;
//        }

//        OpenAndDropContents();
//    }

//    public void OpenAndDropContents()
//    {
//        if (isOpened)
//        {
//            return;
//        }

//        if (!hasGeneratedLoot)
//        {
//            InitializeLoot(true);
//        }

//        isOpened = true;

//        Vector3 spawnPosition = dropPoint != null
//            ? dropPoint.position
//            : transform.position;

//        foreach (ItemInstance item in contents)
//        {
//            if (item == null)
//            {
//                continue;
//            }

//            item.transform.SetParent(null);
//            item.transform.position = spawnPosition;

//            DroppedItem droppedItem = item.gameObject.GetComponent<DroppedItem>();

//            if (droppedItem == null)
//            {
//                droppedItem = item.gameObject.AddComponent<DroppedItem>();
//            }

//            droppedItem.Initialize(item);
//            item.gameObject.SetActive(true);
//        }

//        contents.Clear();
//        MarkOpened();
//    }

//    public void MarkOpened()
//    {
//        isOpened = true;
//    }

//    private static void Shuffle(List<ContainerData.LootEntry> list)
//    {
//        for (int i = list.Count - 1; i > 0; i--)
//        {
//            int randomIndex = Random.Range(0, i + 1);

//            ContainerData.LootEntry temporary = list[i];
//            list[i] = list[randomIndex];
//            list[randomIndex] = temporary;
//        }
//    }
//}