# Match-3

## Voodoo tech assignment
The "Match-3" project was created as part of a technical assignment for Voodoo. Its purpose was to assess my technical skills in building performant, extensible gameplay systems.

## Technical Description

The project was developed in Unity 2022.3.19f1 using C#.
It was tested in portrait in the Unity Editor on Windows.

### Extra Plugins

- DoTween – used for all piece and UI animations (swaps, clears, floating score).
- UniTask – used to manage asynchronous flows (spawning, clearing animations, gravity, timers) in a clean and awaitable manner.
- Unity Addressables – used for loading piece sets and assets in a scalable, memory-efficient way.

### Dependencies

No constant internet dependency is required.
All assets are locally addressable and pooled in memory for runtime performance.

## How to run?

You can run the project:

Inside the Unity Editor (version 2022.3.19f1 or newer).
Build the game for Android directly from the Editor.

## About the Game

The project implements a classic Match-3 gameplay loop:
- Tap or swipe to swap adjacent tiles.
- Form lines of 3 or more to clear them and score points.
- Tiles above fall down with gravity and the board fills up with new tiles from the pool.
- Special pieces like bombs can clear larger areas (e.g. 3×3), rewarding bonus points.
- The player has a time limit, after which the game ends and the score is finalized.

## Systems
- Scoring system (ScoreManager) with configurable rules via ScoreRulesConfig ScriptableObject. Supports combo bonuses, cascade bonuses, and special clears like bombs.
- Match detection – clusters of indices are represented by MatchCluster, which annotates score value and type (normal or special).
- Level configuration – each level is fully defined via a LevelConfig ScriptableObject. Designers can control:
  - Available piece types (availableTypes) – determines which pieces spawn in the level.
  - Grid size (GridWidth, GridHeight) – controls how large the board is.
  - Time allowed (timeForLevel) – sets the timer for the level’s duration.

This allows levels to be created, tweaked, or balanced directly in the Unity Editor without touching code, giving flexibility for fast iteration.

## Cool technical stuff
- Event-driven GameManager – central gameplay logic decoupled from Unity communicates only via async delegates and events (OnPieceSpawnAsync, OnPiecesClearAsync, OnScoreUpdated, etc.).
- MVP/MVVM-inspired presenters decouple game logic from the Unity UI layer (e.g. BoardPresenter, HUDPresenter, GameplayPresenter).
- Factory pattern (PiecePoolFactory) creates and caches reusable piece pools, ensuring only one pool is alive in memory at a time.
- Object Pooling for all game pieces to avoid runtime allocations and improve performance on mobile.
- Asynchronous game flow – animations, spawning, and cascades run in async/await fashion using UniTask.

## TODO
Future improvements I would consider:
- Add more special tiles (striped, wrapped, rainbow piece).
- Expand scoring UI with combo/multiplier feedback.
- Add a level progression system with goals (collect X tiles, reach Y score in Z time).
  
