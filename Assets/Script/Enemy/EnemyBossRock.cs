using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossRock : EnemyAttack
{
    #region 변수
    private Rigidbody EnemyBossRockRigid;
    private float AngularPower = 2f; // 회전 힘
    private float ScaleValue = 0.2f; // 크기
    private bool IsShoot;
    private float Timer = 5f;
    #endregion // 변수

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        EnemyBossRockRigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    private void Update()
    {
        Timer -= Time.deltaTime;

        if(Timer <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    /** 기를 모은다 */
    private IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        IsShoot = true;
    }

    /** 기를 모으는 동안 */
    private IEnumerator GainPower()
    {
        // 차징전까지
        while (!IsShoot)
        {
            AngularPower += 0.02f;
            ScaleValue += 0.005f;
            transform.localScale = Vector3.one * ScaleValue;
            EnemyBossRockRigid.AddTorque(transform.right * AngularPower, ForceMode.Acceleration);
            yield return null;
        }
    }
    #endregion // 함수
}