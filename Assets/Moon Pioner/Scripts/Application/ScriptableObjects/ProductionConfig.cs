using UnityEngine;

[CreateAssetMenu(fileName = "ProductionConfig", menuName = "Settings/ProductionConfig")]
public sealed class ProductionConfig : ScriptableObject
{
    [Header("Production Settings")]
    public float ProductionTime = 5f;
    public float ResourceTranferTime = 1f;

    public ResourceType[] InputResources;
    public ResourceType[] OutputResources;
}