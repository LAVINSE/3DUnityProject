using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StageDataTable : ScriptableObject
{
    [System.Serializable]
    public struct StageInfo
    {
        public string StageName;
        public int StageSpawnActiveCount;
        public int StageEnemyCount;
        public GameObject[] EnemyPrefabList;
        public GameObject EnemyBoss;
    }

    public StageInfo[] StageArray;
}
