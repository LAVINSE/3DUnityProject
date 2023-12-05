using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossMissile : EnemyAttack
{
    #region 변수
    [SerializeField] private Transform Target;

    private NavMeshAgent EnemyBossMissileNavMeshAgent;
    #endregion // 변수

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        EnemyBossMissileNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    /** 초기화 => 상태를 갱신한다 */
    private void Update()
    {
        EnemyBossMissileNavMeshAgent.SetDestination(Target.position);
    }
    #endregion // 함수
}
