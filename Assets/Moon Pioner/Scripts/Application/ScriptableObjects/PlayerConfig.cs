using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Settings/PlayerConfig")]
public sealed class PlayerConfig : ScriptableObject
{
    [Header("Movement Settings")]
    public float MoveSpeed = 5f;

    [Header("Operation Settings")]
    public int StorageLimit = 5;
}