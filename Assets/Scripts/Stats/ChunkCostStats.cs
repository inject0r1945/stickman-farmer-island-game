using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Stats
{
    [CreateAssetMenu(fileName = "ChunkCostStats", menuName = "Chunk Cost Stats")]
    public class ChunkCostStats : ScriptableObject
    {
        [SerializeField] private int[] _costPerLevel;

        public int GetLevelValue(int level)
        {
            if (level >= _costPerLevel.Length)
                return _costPerLevel[_costPerLevel.Length - 1];

            return _costPerLevel[level];
        }
    }
}