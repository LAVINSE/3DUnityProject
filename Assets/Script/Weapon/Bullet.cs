using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region 변수
    [SerializeField] private int BulletDamage;
    #endregion // 변수

    #region 프로퍼티
    public int oBulletDamage
    {
        get => BulletDamage;
        set => BulletDamage = value;
    }
    #endregion // 프로퍼티

    #region 함수
    /** 초기화 => 접촉했을 경우 */
    private void OnCollisionEnter(Collision collision)
    {
        // BulletCase
        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject, 3);
        }
    }

    /** 초기화 => 접촉했을 경우 (트리거) */
    private void OnTriggerEnter(Collider other)
    {
        // Bullet
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
    #endregion // 함수
}
