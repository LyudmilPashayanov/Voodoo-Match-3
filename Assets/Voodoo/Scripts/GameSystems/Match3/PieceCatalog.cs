using System;
using System.Collections.Generic;

namespace Voodoo.Gameplay
{
    public sealed class PieceCatalog
    {
        private readonly PieceType[] _pieceTypes;   // index == typeId
        public int TypeCount => _pieceTypes.Length;
        public readonly int TotalSpawnWeight;

        public PieceCatalog(List<PieceType> defs)
        {
            if (defs == null || defs.Count == 0)
            {
                throw new ArgumentException("No piece types.");
            }
            
            _pieceTypes = new PieceType[defs.Count];
            TotalSpawnWeight = 0;
            for (int i = 0; i < defs.Count; i++)
            {
                _pieceTypes[i] = defs[i];
                if (_pieceTypes[i].AllowRandomSpawn)
                {
                    TotalSpawnWeight +=  defs[i].SpawnWeight;
                }
            }
        }

        public PieceRole RoleOf(int typeId) => _pieceTypes[typeId].Role;
        public bool IsBomb(int typeId) => _pieceTypes[typeId].Role == PieceRole.Bomb;
        public bool AllowRandom(int typeId) => _pieceTypes[typeId].AllowRandomSpawn;
        public int WeightOf(int typeId) => Math.Max(0, _pieceTypes[typeId].SpawnWeight);
    }
}