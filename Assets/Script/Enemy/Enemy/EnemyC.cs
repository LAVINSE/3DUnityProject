using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyC : Enemy
{
    #region 변수
    [Space]
    [Header("=====> 적 원거리 <=====")]
    [SerializeField] private GameObject EnemyBullet;
    [SerializeField] private float TargetRadius = 0.5f;


    #endregion // 변수

    #region 함수
    /** 초기화 */
    protected override void Awake()
    {
        base.Awake();
    }

    /** 초기화 => 상태를 갱신한다 */
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    /** 플레이어를 타겟한다 */
    public override void Targeting()
    {
        // 구체모양의 레이캐스트 (모든오브젝트)
        RaycastHit[] RayHitArray = Physics.SphereCastAll(this.transform.position, TargetRadius,
            transform.forward, oAttackRange, TargetLayer);

        // 플레이어가 있을 경우, 공격중 X
        if (RayHitArray.Length > 0 && !IsAttack)
        {
            StartCoroutine(EnemyAttack());
        }
    }

    /** 적 공격 */
    private IEnumerator EnemyAttack()
    {
        IsTracking = false;
        IsAttack = true;
        EnemyAnimator.SetBool("IsAttack", true);

        yield return new WaitForSeconds(0.5f);
        GameObject EnemyBulletObject = Instantiate(EnemyBullet, transform.position, transform.rotation);
        Rigidbody EnemyBulletRigid = EnemyBulletObject.GetComponent<Rigidbody>();
        EnemyBulletRigid.velocity = transform.forward * 20;
        yield return new WaitForSeconds(1f);

        IsTracking = true;
        IsAttack = false;
        EnemyAnimator.SetBool("IsAttack", false);
    }
    #endregion // 함수
}
