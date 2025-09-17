using UnityEngine.AddressableAssets;

namespace Voodoo.ConfigScriptableObjects
{
    [System.Serializable]
    public class LevelEntry
    {
        public int LevelId;
        public AssetReferenceT<LevelConfig> PieceSetRef;
    }
}