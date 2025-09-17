using System;
using UnityEngine;
using Voodoo.Gameplay.Core;

namespace Voodoo.Gameplay
{
    [Serializable]
    public struct PieceType
    {
        public PieceRole Role;
        [Range(1,100)] public int SpawnWeight; // 100 meaning it will be spawned very often, 1 meaning it will be spawned very rarely.
        public bool AllowRandomSpawn;
    }
}