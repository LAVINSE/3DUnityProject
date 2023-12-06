using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    #region 변수
    [SerializeField] private int AttackDamage;
    [SerializeField] private bool IsMelee;
    #endregion // 변수

    #region 프로퍼티
    public int oAttackDamage
    {
        get => AttackDamage;
        set => AttackDamage = value;
    }
    #endregion // 프로퍼티

    #region 함수
    /** 초기화 => 접촉했을 경우 (트리거) */
    private void OnTriggerEnter(Collider other)
    {
        // Bullet
        if (!IsMelee && other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
    #endregion // 함수
}
