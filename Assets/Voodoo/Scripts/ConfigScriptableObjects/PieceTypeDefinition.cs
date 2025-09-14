using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "PieceType", menuName = "Match3/Piece Type", order = 0)]
public class PieceTypeDefinition : ScriptableObject
{
    [Tooltip("Unique string ID used to identify this piece type")]
    public string id;

    [Tooltip("Prefab used for rendering this piece")]
    public AssetReferenceGameObject prefabReference;

    [Header("Spawn Rules")]
    public PieceType pieceType = new PieceType() {Role = PieceRole.Normal, SpawnWeight = 100, AllowRandomSpawn = true};
}

[Serializable]
public struct PieceType
{
    public PieceRole Role;
    [Range(1,100)] public int SpawnWeight; // 100 meaning it will be spawned very often, 1 meaning it will be spawned very rarely.
    public bool AllowRandomSpawn;
}