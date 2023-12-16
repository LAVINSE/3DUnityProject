using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyB : Enemy
{
    #region 변수
    [Space]
    [Header("=====> 적 근접공격 <=====")]
    [SerializeField] private BoxCollider EnemyAttackMeleeBoxCollider;
    [SerializeField] private float TargetRadius = 1f;
    [SerializeField] private float TargetRange = 12f;


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
        Targeting();
    }

    /** 플레이어를 타겟한다 */
    public override void Targeting()
    {
        // 구체모양의 레이캐스트 (모든오브젝트)
        RaycastHit[] RayHitArray = Physics.SphereCastAll(this.transform.position, TargetRadius,
            transform.forward, TargetRange, LayerMask.GetMask("Player"));

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

        yield return new WaitForSeconds(0.1f);
        EnemyRigid.AddForce(transform.forward * 20, ForceMode.Impulse);
        EnemyAttackMeleeBoxCollider.enabled = true;

        yield return new WaitForSeconds(0.5f);
        EnemyRigid.velocity = Vector3.zero;
        EnemyAttackMeleeBoxCollider.enabled = false;
        yield return new WaitForSeconds(2f);

        IsTracking = true;
        IsAttack = false;
        EnemyAnimator.SetBool("IsAttack", false);
    }
    #endregion // 함수
}
