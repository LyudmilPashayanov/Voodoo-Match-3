using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Voodoo.ConfigScriptableObjects;
using Voodoo.Gameplay.Core;
using Voodoo.GameSystems.Utilities;

namespace Voodoo.Gameplay
{
    public sealed class GameFlow : IGameFlow, IDisposable
    {
        private GameManager _gameManager;
        
        private readonly AssetReferenceT<LevelConfig> _setReference;
        private readonly PiecePoolFactory _poolFactory;


        private AsyncOperationHandle<LevelConfig> _setHandle;
        
        private LevelConfig _set;
        private PieceTypeDefinition[] _availableTypes; // index == typeId used by GameManager
        private ScoreRulesConfig _scoreRulesConfig;

        
        private PiecePool _pool;
        public IPiecePool Pool => _pool;
        
        public Func<int, PieceTypeDefinition, UniTask> PieceSpawnAsync { get; set; }
        public Func<IReadOnlyList<MatchCluster>,UniTask> PiecesClearAsync { get; set; } 
        public Func<int, int, UniTask> PieceSwapAsync { get; set; }
        public Func<int, int, UniTask> NoMatchSwapAsync { get; set; }
        public Func<UniTask> InvalidMoveAsync { get; set; }
        public Func<IReadOnlyList<(int from, int to)>, UniTask> OnGravityMovesAsync { get; set; }
        public event Action<int> ScoreChanged;
        public event Action<int> TimeChanged;
        public event Action GameOver;
        public event Action<int, int> GameLoaded;
        
        public GameFlow(AssetReferenceT<LevelConfig> setReference, PiecePoolFactory piecePoolFactory, ScoreRulesConfig scoreRulesConfig)
        {
            _setReference = setReference;
            _poolFactory = piecePoolFactory;
            _scoreRulesConfig = scoreRulesConfig;
        }

        /// <summary>
        /// After loading assets and pool, creates the GameManager and starts ot. 
        /// </summary>
        public async UniTask StartGameAsync(CancellationToken ct = default)
        {
            if (_gameManager != null) return;
            
            await PrepareAsync(ct);
            
            _availableTypes = _set.availableTypes;
            
            await UniTask.SwitchToMainThread(ct);
            
            PieceCatalog catalog = GeneratePieceCatalog(_availableTypes);
            ScoreRulesData scoreRulesData = GenerateScoreRulesData(_scoreRulesConfig);
            
            _gameManager = new GameManager(_set.GridWidth, _set.GridHeight, catalog, _set.timeForLevel, scoreRulesData);

            WireModelEvents(_gameManager);
    
            GameLoaded?.Invoke(_set.GridWidth, _set.GridHeight);
            
            _gameManager.StartGame();
        }

        private ScoreRulesData GenerateScoreRulesData(ScoreRulesConfig scoreRulesConfig)
        {
            return new ScoreRulesData
            {
                PointsPerTile = scoreRulesConfig.pointsPerTile,
                BonusFor3 = scoreRulesConfig.bonusFor3,
                BonusFor4 = scoreRulesConfig.bonusFor4,
                BonusFor5 = scoreRulesConfig.bonusFor5,
                CascadeBonusPerLevel = scoreRulesConfig.cascadeBonusPerLevel,
                BombClearBonus = scoreRulesConfig.bombClearBonus
            };
        }

        private PieceCatalog GeneratePieceCatalog(PieceTypeDefinition[] setToUse)
        {
            List<PieceType> types = new List<PieceType>();
            foreach (var type in setToUse)
            {
                types.Add(type.pieceType);
            }
            
            return new PieceCatalog(types);
        }
        
        /// <summary>
        /// Loads any addressable assets and creates the Pool if needed.
        /// </summary>
        private async UniTask PrepareAsync(CancellationToken ct = default)
        {
            // Load the set
            _setHandle = _setReference.LoadAssetAsync();
            await _setHandle.ToUniTask(cancellationToken: ct);
            if (_setHandle.Status != AsyncOperationStatus.Succeeded)
            {
                throw new System.Exception("Failed to load PieceSetConfig");
            }
            
            _set = _setHandle.Result;
    
            _pool = await _poolFactory.GetOrCreateAsync(_set, ct);
        }
        
        public void EndGame()
        {
            if (_gameManager != null)
            {
                _gameManager.EndGame();
                UnwireModelEvents(_gameManager);
                _gameManager = null;
            }
            
            _pool = null;

            // Release set
            if (_setHandle.IsValid())
            {
                Addressables.Release(_setHandle);
                _setHandle = default;
            }

            _set = null;
        }
        
        /// <summary>
        /// Matches the id spawned in the GameManager to a Unity PieceType so that UI can work.
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <param name="typeId"></param>
        private async UniTask OnModelPieceSpawnedAsync(int cellIndex, int typeId)
        {
            if ((uint)typeId >= (uint)_availableTypes.Length)
            {
                return;
            }
            
            await PieceSpawnAsync(cellIndex, _availableTypes[typeId]);
        }
        private async UniTask OnModelPiecesClearedAsync(IReadOnlyList<MatchCluster> clusterToClear)
        {
            await PiecesClearAsync(clusterToClear);
        }

        private async UniTask OnModelSwappedAsync(int from, int to)
        {
            await PieceSwapAsync(from, to);
        }
        
        private async UniTask OnModelNoMatchSwap(int from, int to)
        {
            await NoMatchSwapAsync(from, to);
        }
        
        private async UniTask OnModelInvalidMove()
        {
            await InvalidMoveAsync();
        }
        
        private async UniTask GravityMovesAsync(IReadOnlyList<(int from, int to)> listOfMoves)
        {
            await OnGravityMovesAsync(listOfMoves);
        }
        
        private void OnModelScoreUpdated(int score)
        {
            ScoreChanged?.Invoke(score);
        }

        private void OnModelTimeChanged(int time)
        {
            TimeChanged?.Invoke(time);
        }

        private void OnModelGameOver()
        {
            GameOver?.Invoke();
        }
        
        private void WireModelEvents(GameManager gm)
        {
            gm.OnPieceSpawnAsync = OnModelPieceSpawnedAsync;
            gm.OnPiecesClearAsync = OnModelPiecesClearedAsync;
            gm.OnSwapCommittedAsync = OnModelSwappedAsync;
            gm.OnNoMatchSwapAsync = OnModelNoMatchSwap;
            gm.OnInvalidMoveAsync = OnModelInvalidMove;
            gm.OnGravityMovesAsync = GravityMovesAsync;
            gm.OnScoreUpdated += OnModelScoreUpdated;
            gm.OnTimeChanged += OnModelTimeChanged;
            gm.OnGameOver += OnModelGameOver;
        }

        private void UnwireModelEvents(GameManager gm)
        {
            gm.OnPieceSpawnAsync = null;
            gm.OnPiecesClearAsync = null;
            gm.OnSwapCommittedAsync = null;
            gm.OnNoMatchSwapAsync = null;
            gm.OnInvalidMoveAsync = null;
            gm.OnGravityMovesAsync = null;
            gm.OnScoreUpdated -= OnModelScoreUpdated;
            gm.OnTimeChanged -= OnModelTimeChanged;
            gm.OnGameOver -= OnModelGameOver;
        }

        public void PieceClicked(int pieceClickedIndex)
        {
            _ = _gameManager?.ClickPiece(pieceClickedIndex);
        }
        
        public void SwapPiece(int pieceClickedIndex, Direction direction)
        {
            _ = _gameManager?.SwipePiece(pieceClickedIndex, direction);
        }

        public void Tick(float deltaTime)
        {
            _gameManager?.TickTime(deltaTime);
        }
        
        public void Pause()
        {
            _gameManager?.PauseTimer();
        }

        public void Resume()
        {
            _gameManager?.ResumeTimer();
        }
        
        public void Dispose()
        {
            EndGame();
        }
    }
}