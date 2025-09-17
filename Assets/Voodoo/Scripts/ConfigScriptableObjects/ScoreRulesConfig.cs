using UnityEngine;

namespace Voodoo.ConfigScriptableObjects
{
    [CreateAssetMenu(fileName = "ScoreRulesConfig", menuName = "Match3/ScoreRulesConfig", order = 1)]
    public class ScoreRulesConfig : ScriptableObject
    {
        [Header("Base Points")]
        public int pointsPerTile = 10;

        [Header("Run Bonuses")]
        public int bonusFor3 = 0;
        public int bonusFor4 = 50;
        public int bonusFor5 = 150;

        [Header("Cascade Bonus")]
        public int cascadeBonusPerLevel = 50;

        [Header("Specials")]
        public int bombClearBonus = 200; // example, expand later
    }
}