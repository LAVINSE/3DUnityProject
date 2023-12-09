using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBoss : Enemy
{
    #region 변수
    [SerializeField] private GameObject EnemyBossMissilePrefab;
    [SerializeField] private GameObject EnemyBossRockPrefab;
    [SerializeField] private Transform EnemyBossMissilePortA;
    [SerializeField] private Transform EnemyBossMissilePortB;
    [SerializeField] private BoxCollider EnemyBossTauntBoxCollider;
    [SerializeField] private bool IsLook;

    private Vector3 LookVector; // 바라보는 방향 예측
    private Vector3 TauntVector;
    #endregion // 변수

    #region 함수
    /** 초기화 */
    protected override void Awake()
    {
        base.Awake();

        EnemyNavMeshAgent.isStopped = true;
        IsLook = true;

        StartCoroutine(Select());
    }

    /** 초기화 => 상태를 갱신한다 */
    private void Update()
    {
        // 적이 죽었을 경우
        if (IsEnemyDead)
        {
            StopAllCoroutines();
            return;
        }

        // 바라보기가 true일때
        if (IsLook)
        {
            float Horizontal = Input.GetAxisRaw("Horizontal");
            float Vertical = Input.GetAxisRaw("Vertical");
            LookVector = new Vector3(Horizontal, 0, Vertical) * 5f;
            transform.LookAt(Target.position + LookVector);
        }
        else
        {
            EnemyNavMeshAgent.SetDestination(TauntVector);
        }
    }

    private IEnumerator Select()
    {
        yield return new WaitForSeconds(0.1f);

        int RandomAction = Random.Range(0, 5);

        switch(RandomAction)
        {
            case 0:
            case 1:
                // 미사일
                StartCoroutine(EnemyBossMissileShot());
                break;
            case 2:
            case 3:
                // 돌 굴리기
                StartCoroutine(EnemyBossRockShot());
                break;
            case 4:
                // 점프 공격
                StartCoroutine(EnemyBossTaunt());
                break;
        }
    }

    /** 적 보스 미사일 공격 */
    private IEnumerator EnemyBossMissileShot()
    {
        EnemyAnimator.SetTrigger("TriggerShot");
        yield return new WaitForSeconds(0.2f);
        GameObject EnemyBossMissileObjectA = Instantiate(EnemyBossMissilePrefab, EnemyBossMissilePortA.position,
            EnemyBossMissilePortA.rotation);
        EnemyBossMissile BossMissileA = EnemyBossMissileObjectA.GetComponent<EnemyBossMissile>();
        BossMissileA.oTarget = Target;

        yield return new WaitForSeconds(0.3f);
        GameObject EnemyBossMissileObjectB = Instantiate(EnemyBossMissilePrefab, EnemyBossMissilePortB.position,
            EnemyBossMissilePortB.rotation);
        EnemyBossMissile BossMissileB = EnemyBossMissileObjectB.GetComponent<EnemyBossMissile>();
        BossMissileB.oTarget = Target;

        yield return new WaitForSeconds(2f);

        StartCoroutine(Select());
    }

    /** 적 보스 돌 굴리기 공격 */
    private IEnumerator EnemyBossRockShot()
    {
        IsLook = false;
        EnemyAnimator.SetTrigger("TriggerBigShot");
        Instantiate(EnemyBossRockPrefab, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        IsLook = true;
        StartCoroutine(Select());
    }

    /** 적 보스 점프 찍기 공격 */
    private IEnumerator EnemyBossTaunt()
    {
        TauntVector = Target.position + LookVector;

        IsLook = false;
        EnemyNavMeshAgent.isStopped = false;
        EnemyBoxCollider.enabled = false;
        EnemyAnimator.SetTrigger("TriggerTaunt");
        yield return new WaitForSeconds(1.5f);
        EnemyBossTauntBoxCollider.enabled = true;

        yield return new WaitForSeconds(0.5f);
        EnemyBossTauntBoxCollider.enabled = false;

        yield return new WaitForSeconds(1f);
        IsLook = true;
        EnemyNavMeshAgent.isStopped = true;
        EnemyBoxCollider.enabled = true;  
        StartCoroutine(Select());
    }
    #endregion // 함수
}