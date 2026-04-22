using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "LocalizationCache", menuName = "Settings/LocalizationCache")]
public sealed class LocalizationCache : ScriptableObject
{
    [Header("Player")]
    public LocalizedString PlayerNotEnoughtSpace;

    [Header("Storage")]
    public LocalizedString StorageNotEnoughtSpace;

    [Header("Production")]
    public LocalizedString ProductionStopNoResouces;

    public LocalizedString ProductionStopNoSpace;

    public LocalizedString ProductionStopUnexpected;
}