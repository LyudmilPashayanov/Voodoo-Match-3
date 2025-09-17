using UnityEngine;
using UnityEngine.AddressableAssets;
using Voodoo.Gameplay;
using Voodoo.Gameplay.Core;

namespace Voodoo.ConfigScriptableObjects
{
    [CreateAssetMenu(fileName = "PieceType", menuName = "Match3/Piece Type", order = 0)]
    public class PieceTypeDefinition : ScriptableObject
    {
        [Tooltip("Unique string ID used to identify this piece type")]
        public string id;

        [Tooltip("Prefab used for rendering this piece")]
        public AssetReferenceGameObject prefabReference;

        [Header("Spawn Rules")] public PieceType pieceType = new PieceType()
            { Role = PieceRole.Normal, SpawnWeight = 100, AllowRandomSpawn = true };
    }
}