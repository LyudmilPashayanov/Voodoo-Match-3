namespace Voodoo.Gameplay.Core
{
    /// <summary>
    /// Used in the  <see cref="ScoreManager"/> to decide what score and bonuses to give for different matches.
    /// </summary>
    public struct ScoreRulesData
    {
        public int PointsPerTile;
        public int BonusFor3;
        public int BonusFor4;
        public int BonusFor5;
        public int CascadeBonusPerLevel;
        public int BombClearBonus;
    }
}