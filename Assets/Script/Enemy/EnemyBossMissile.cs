using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossMissile : EnemyAttack
{
    #region 변수
    [SerializeField] private Transform Target;
    private float Timer = 5f;

    private NavMeshAgent EnemyBossMissileNavMeshAgent;
    #endregion // 변수

    #region 프로퍼티
    public Transform oTarget
    {
        get => Target;
        set => Target = value;
    }
    #endregion // 프로퍼티

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

        Timer -= Time.deltaTime;

        if (Timer <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion // 함수
}
