using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossA : Enemy
{
    #region 변수
    [SerializeField] private bool IsLook;
    [SerializeField] private GameObject EnemyBossMissilePrefab;
    [SerializeField] private GameObject EnemyBossRockPrefab;
    [SerializeField] private Transform EnemyBossMissilePortA;
    [SerializeField] private Transform EnemyBossMissilePortB;
    [SerializeField] private BoxCollider EnemyBossTauntBoxCollider;
    [SerializeField] private Transform RockSpawnPos;
    [SerializeField] private float TargetRadius;

    private Vector3 LookVector; // 바라보는 방향 예측
    private Vector3 TauntVector;
    #endregion // 변수

    #region 함수
    /** 초기화 */
    protected override void Awake()
    {
        base.Awake();

        IsLook = true;   
    }

    /** 초기화 => 상태를 갱신한다 */
    protected override void Update()
    {
        base.Update();

        // 바라보기가 true일때
        if (IsLook)
        {
            float Horizontal = Input.GetAxisRaw("Horizontal");
            float Vertical = Input.GetAxisRaw("Vertical");
            LookVector = new Vector3(Horizontal, 0, Vertical) * 5f;
            transform.LookAt(PlayerTarget.position + LookVector);
        }
        else
        {
            oEnemyNavMeshAgent.SetDestination(TauntVector);
        }
    }

    /** 타겟한다 */
    public override void Targeting()
    {
        base.Targeting();
        // 구체모양의 레이캐스트 (모든오브젝트)
        RaycastHit[] RayHitArray = Physics.SphereCastAll(this.transform.position, TargetRadius,
            transform.forward, oAttackRange, TargetLayer);

        // 플레이어가 있을 경우, 공격중 X
        if (RayHitArray.Length > 0 && !IsAttack)
        {
            StartCoroutine(Select());
        }
        
    }

    private IEnumerator Select()
    {
        IsAttack = true;
        yield return new WaitForSeconds(0.1f);
        IsTracking = false;

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
        BossMissileA.oTarget = PlayerTarget;

        yield return new WaitForSeconds(0.3f);
        GameObject EnemyBossMissileObjectB = Instantiate(EnemyBossMissilePrefab, EnemyBossMissilePortB.position,
            EnemyBossMissilePortB.rotation);
        EnemyBossMissile BossMissileB = EnemyBossMissileObjectB.GetComponent<EnemyBossMissile>();
        BossMissileB.oTarget = PlayerTarget;

        yield return new WaitForSeconds(2f);

        IsTracking = true;
        IsAttack = false;
    }

    /** 적 보스 돌 굴리기 공격 */
    private IEnumerator EnemyBossRockShot()
    {
        yield return new WaitForSeconds(1.5f);
        EnemyAnimator.SetTrigger("TriggerBigShot");

        var Rock = Instantiate(EnemyBossRockPrefab, RockSpawnPos.position, transform.rotation);

        yield return new WaitForSeconds(2f);

        IsLook = true;
        IsTracking = true;
        IsAttack = false;
    }

    /** 적 보스 점프 찍기 공격 */
    private IEnumerator EnemyBossTaunt()
    {
        TauntVector = PlayerTarget.position + LookVector;

        EnemyBoxCollider.enabled = false;

        IsLook = false;
        
        EnemyAnimator.SetTrigger("TriggerTaunt");
        yield return new WaitForSeconds(1.5f);
        EnemyBossTauntBoxCollider.enabled = true;

        yield return new WaitForSeconds(0.5f);
        EnemyBossTauntBoxCollider.enabled = false;

        yield return new WaitForSeconds(1f);
        IsLook = true;

        IsTracking = true;
        IsAttack = false;

        EnemyBoxCollider.enabled = true;  
    }
    #endregion // 함수
}
