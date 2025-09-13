using UnityEngine;
using UnityEngine.AddressableAssets;
using Voodoo.ConfigScriptableObjects;
using Voodoo.Gameplay;
using Voodoo.UI;

namespace Voodoo
{
    /// <summary>
    /// This class is the first thing that runs in the game.
    /// It is responsible to create the classes in the required order
    /// and if needed to inject them with one another, so that they can
    /// hold the references they need.
    /// /// </summary>
    public class BootManager : MonoBehaviour
    {
        [SerializeField] private AssetReferenceT<PieceSetConfig> pieceSetConfigRef;
        [SerializeField] private UIManager _UIManager;

        public void Start()
        {
            GameFlow gameFlow = new GameFlow(pieceSetConfigRef);
            _UIManager.Init(gameFlow);
        }
    }
}
