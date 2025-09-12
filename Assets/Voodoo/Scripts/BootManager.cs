using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Voodoo.Gameplay;
using Voodoo.UI;
using Grid = Voodoo.Gameplay.Grid;

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

        private void Start()
        {
            pieceSetConfigRef.LoadAssetAsync().Completed += OnPieceSetLoaded;
            // Maybe in the future I wiill have to load other data?
        }

        private void OnPieceSetLoaded(AsyncOperationHandle<PieceSetConfig> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                PieceSetConfig config = handle.Result;

                Init(config);
            }
            else
            {
                Debug.LogError("Failed to load PieceSetConfig");
            }
        }

        private void Init(PieceSetConfig pieceSetConfig)
        {
            Grid grid = new Grid(5, 5, pieceSetConfig.availableTypes);

            GameManager gameManager = new GameManager(grid);
           
            _UIManager.Init(gameManager);

            //gameManager.StartGame();
        }
    }
}
