using UnityEngine;

[CreateAssetMenu(fileName = "ThrowableItemData", menuName = "Scriptable Objects/ItemData/ThrowableItemData")]
public class ThrowableItemData : ItemData
{
    [SerializeField]private float throwForce;
    [SerializeField]private float upwardForce;
    [SerializeField]private float effectDuration;
    [SerializeField]private float noiseRadius;
    public void CreateThrowConfig()
    {

    }
}
