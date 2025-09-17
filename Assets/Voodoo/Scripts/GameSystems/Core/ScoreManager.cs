using System.Collections.Generic;

namespace Voodoo.Gameplay.Core
{
    public sealed class ScoreManager
    {
        
        private readonly ScoreRulesData _rules;
        private int _currentScore;

        public int CurrentScore => _currentScore;

        public ScoreManager(ScoreRulesData rules)
        {
            _rules = rules;
            _currentScore = 0;
        }

        /// <summary>
        /// Adds score for the given clusters and cascade level.
        /// Annotates clusters with their score value.
        /// </summary>
        public int AddClusters(IReadOnlyList<MatchCluster> clusters, int cascadeLevel)
        {
            int total = 0;

            foreach (var cluster in clusters)
            {
                int clusterScore = CalculateClusterScore(cluster.Length, cascadeLevel);

                // annotate cluster with the points it gave
                cluster.ScoreValue = clusterScore;

                total += clusterScore;
            }

            _currentScore += total;
            return total;
        }

        /// <summary>
        /// Pure scoring function for one cluster.
        /// </summary>
        private int CalculateClusterScore(int length, int cascadeLevel)
        {
            int score = length * _rules.PointsPerTile;

            if (length >= 5) score += _rules.BonusFor5;
            else if (length == 4) score += _rules.BonusFor4;
            else if (length == 3) score += _rules.BonusFor3;

            score += cascadeLevel * _rules.CascadeBonusPerLevel;

            return score;
        }

        public void Reset()
        {
            _currentScore = 0;
        }
    }
}