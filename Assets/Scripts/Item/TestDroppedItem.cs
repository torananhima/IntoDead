using UnityEngine;

public class TestDroppedItem : MonoBehaviour
{
    [SerializeField] ItemData data;

    private void Start()
    {
        DroppedItem item = GetComponent<DroppedItem>();
        ItemInstance instance = new ItemInstance(data, 1);
        item.Initialize(instance);
    }
}
