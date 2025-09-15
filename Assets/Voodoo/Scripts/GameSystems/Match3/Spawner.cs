using System;
using System.Collections.Generic;
using Voodoo.Gameplay;

namespace Voodoo.Scripts.GameSystems
{
    public sealed class Spawner
    {
        private readonly int[] _availableIds;
        private readonly int[] _weights; // based on the _availableIds
        private readonly int[] _cumulativeWeights; // based on the _availableIds
        private readonly int _totalSpawnWeight;

        public Spawner(PieceCatalog catalog)
        {
            var ids = new List<int>(catalog.TypeCount);
            var wts = new List<int>(catalog.TypeCount);
            var cum = new List<int>(catalog.TypeCount);
            int totalWeight = 0;
            
            for (int t = 0; t < catalog.TypeCount; t++)
            {
                if (!catalog.AllowRandom(t))
                {
                    continue;
                }
                int spawnWeight = catalog.WeightOf(t);
                if (spawnWeight <= 0)
                {
                    continue;
                }
                ids.Add(t);
                wts.Add(spawnWeight);
                totalWeight += spawnWeight;
                cum.Add(totalWeight);
            }

            if (totalWeight <= 0)
            {
                throw new InvalidOperationException("No spawnable types.");
            }
            
            _availableIds = ids.ToArray(); 
            _weights = wts.ToArray();
            _cumulativeWeights = cum.ToArray();
            _totalSpawnWeight = totalWeight;
        }

        private int PickPiece(Random rng)
        {
            int randomWeight = rng.Next(1, _totalSpawnWeight + 1);
            return SelectByBinarySearch(randomWeight, _cumulativeWeights, _availableIds);
        }

        /// <summary>
        /// Pick a random piece based on a filter function provided.
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="filterFunctionByIndex"></param>
        /// <returns></returns>
        public int PickPieceWithFilter(Random rng, Func<int, bool> filterFunctionByIndex)
        {
            // Temporary stack arrays (fast, no heap GC)
            Span<int> candidatesIds = stackalloc int[_availableIds.Length];   // stores typeIds that pass the given filter
            Span<int> cumulative = stackalloc int[_availableIds.Length];   // cumulative weight sums

            int allowedCount = 0;   // how many allowed types we’ve kept
            int filteredTotalWeight = 0;   // sum of weights of allowed types
            
            for (int i = 0; i < _availableIds.Length; i++)
            {
                /*
                int weight = _weights[i];
                if (weight <= 0) // Skip if weight <= 0
                {
                    continue;
                }
                */
                
                int typeId = _availableIds[i];
                if (!filterFunctionByIndex(typeId)) // Skip if filter says this type is not allowed
                {
                    continue;
                }

                // Add this type’s weight to running total
                filteredTotalWeight += _weights[i];

                // Save this typeId in allowed list
                candidatesIds[allowedCount] = typeId;

                // Save the cumulative sum (example: [10, 25, 40])
                cumulative[allowedCount] = filteredTotalWeight;
                
                allowedCount++;
            }

            if (allowedCount == 0) // No allowed types, fallback to normal pick
            {
                return PickPiece(rng);
            }
            
            int roll = rng.Next(1, filteredTotalWeight + 1); // random number in [1..totalWeight]
          
            return SelectByBinarySearch(roll, cumulative, candidatesIds);
        }
        
        private int SelectByBinarySearch(int roll, Span<int> cumulative, Span<int> candidates)
        {
            int left = 0;
            int right = candidates.Length - 1;

            while (left < right)
            {
                int midIndex = (left + right) / 2;

                if (roll <= cumulative[midIndex])
                {
                    right = midIndex;  // look left half
                }
                else
                {
                    left = midIndex + 1; // look right half
                }
            }

            return candidates[left];
        }
    }
}
