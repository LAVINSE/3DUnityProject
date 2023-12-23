using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        Melee,
        Range,
        Magic,
    }

    #region 변수
    [Header("=====> 무기 타입 <=====")]
    [SerializeField] private WeaponType Type;

    [Space]
    [Header("=====> 근접무기 설정 <=====")]
    [SerializeField] private int WeaponMeleeDamage;
    [SerializeField] private BoxCollider MeleeArea;
    [SerializeField] TrailRenderer MeleeTrailEffect;

    [Space]
    [Header("=====> 원거리무기 설정 <=====")]
    [SerializeField] private Transform BulletPos;
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private Transform BulletCasePos;
    [SerializeField] private GameObject BulletCasePrefab;
    [SerializeField] private int MaxAmmo;
    [SerializeField] private int CurrentAmmo;

    [Space]
    [Header("=====> 무기 공통 설정 <=====")]
    [SerializeField] private float WeaponRate;
    [SerializeField] private float MagicRate;
    
    #endregion // 변수

    #region 프로퍼티
    public float oWeaponRate
    {
        get => WeaponRate;
        set => WeaponRate = value;
    }
    public float oMagicRate => MagicRate;
    public int oCurrentAmmo
    {
        get => CurrentAmmo;
        set => CurrentAmmo = value;
    }
    public int oMaxAmmo
    {
        get => MaxAmmo;
        set => MaxAmmo = value;
    }
    public int oWeaponMeleeDamage
    {
        get => WeaponMeleeDamage;
        set => WeaponMeleeDamage = value;
    }
    public WeaponType oType
    {
        get => Type;
        set => Type = value;
    }
    #endregion // 프로퍼티

    #region 함수
    /** 무기를 사용한다 */
    public void WeaponUse()
    {
        // 근접무기
        if(Type == WeaponType.Melee)
        {
            StopCoroutine(MeleeSwing());
            StartCoroutine(MeleeSwing());
        }
        // 원거리무기, 탄약이 남았을경우
        else if(Type == WeaponType.Range && CurrentAmmo > 0)
        {
            CurrentAmmo--;
            StartCoroutine(RangeShot());
        }
    }

    /** 마법을 사용한다 */
    public void WeaponMagic()
    {
        // 근접무기
        if (Type == WeaponType.Melee)
        {
            StopCoroutine(MagicShot());
            StartCoroutine(MagicShot());
        }
    }

    /** 근접무기 스윙 */
    private IEnumerator MeleeSwing()
    {
        yield return new WaitForSeconds(0.4f);
        MeleeArea.enabled = true;
        MeleeTrailEffect.enabled = true;

        yield return new WaitForSeconds(0.1f);
        MeleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        MeleeTrailEffect.enabled = false;
    }

    /** 원거리무기 총 */
    private IEnumerator RangeShot()
    {
        // 총알 생성
        GameObject Bullet = Instantiate(BulletPrefab, BulletPos.position, BulletPos.rotation);
        Rigidbody BulletRigid = Bullet.GetComponent<Rigidbody>();

        // 총알 속도
        BulletRigid.velocity = BulletPos.forward * 50;

        yield return null;

        // 탄피 생성
        GameObject BulletCase = Instantiate(BulletCasePrefab, BulletCasePos.position, BulletCasePos.rotation);
        Rigidbody BulletCaseRigid = BulletCase.GetComponent<Rigidbody>();

        // 탄피 튀는 현상
        Vector3 BulletCaseVector = Vector3.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        BulletCaseRigid.AddForce(BulletCaseVector, ForceMode.Impulse);
        BulletCaseRigid.AddTorque(Vector3.up);
    }

    /** 마법무기 */
    private IEnumerator MagicShot()
    {
        yield return null;
    }
    #endregion // 함수
}
