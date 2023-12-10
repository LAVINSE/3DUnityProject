using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    #region 변수
    [Header("=====> 적 정보 <=====")]
    [SerializeField] private int MaxHealth;
    [SerializeField] private int CurrentHealth;

    [Space]
    [Header("=====> 적 공통 <=====")]
    [SerializeField] protected Transform Target;
    [SerializeField] protected bool IsTracking;
    [SerializeField] protected bool IsAttack;  
    
    protected Rigidbody EnemyRigid;
    protected BoxCollider EnemyBoxCollider;
    protected MeshRenderer[] EnemyMeshArray;
    protected NavMeshAgent EnemyNavMeshAgent;
    protected Animator EnemyAnimator;

    protected bool IsEnemyDead;
    #endregion // 변수

    #region 프로퍼티
    public int oMaxHealth
    {
        get => MaxHealth;
        set => MaxHealth = value;
    }
    public int oCurrentHealth
    {
        get => CurrentHealth;
        set => CurrentHealth = value;
    }
    #endregion // 프로퍼티

    #region 함수
    /** 초기화 */
    protected virtual void Awake()
    {
        EnemyRigid = GetComponent<Rigidbody>();
        EnemyBoxCollider = GetComponent<BoxCollider>();
        EnemyMeshArray = GetComponentsInChildren<MeshRenderer>();
        EnemyNavMeshAgent = GetComponent<NavMeshAgent>();
        EnemyAnimator = GetComponentInChildren<Animator>();

        Invoke("TrackingStart", 2);
    }

    /** 초기화 => 상태를 갱신한다 */
    protected virtual void FixedUpdate()
    {
        FreezeVelocity();
    }

    /** 초기화 => 접촉했을경우 (트리거) */
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Melee"))
        {
            Weapon WeaponComponent = other.gameObject.GetComponent<Weapon>();

            // 체력감소
            CurrentHealth -= WeaponComponent.oWeaponMeleeDamage;
            // 넉백계산
            Vector3 ReactVec = transform.position - other.transform.position;

            StartCoroutine(OnHit(ReactVec, false));
            Debug.Log($"근접공격 체력 : {CurrentHealth}");
        }
        else if (other.gameObject.CompareTag("Bullet"))
        {
            Bullet BulletComponent = other.gameObject.GetComponent<Bullet>();

            // 체력감소
            CurrentHealth -= BulletComponent.oBulletDamage;
            // 넉백계산
            Vector3 ReactVector = transform.position - other.transform.position;

            // 총알 삭제
            Destroy(other.gameObject);
            StartCoroutine(OnHit(ReactVector, false));
            Debug.Log($"원거리공격 체력 : {CurrentHealth}");
        }
    }

    /** 수류탄 피격 */
    public void HitGrenade(Vector3 ExplosionPos)
    {
        // TODO : 데미지변수 받아오기 수정필요
        // 체력감소
        CurrentHealth -= 100;
        Debug.Log($"수류탄 체력 : {CurrentHealth}");
        Vector3 ReactVector = this.transform.position - ExplosionPos;
        StartCoroutine(OnHit(ReactVector, true));
    }

    /** 적 피격효과 */
    private IEnumerator OnHit(Vector3 ReactVector, bool IsGrenade)
    {
        foreach(MeshRenderer Mesh in EnemyMeshArray)
        {
            Mesh.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.1f);

        // 살아있을 경우
        if(CurrentHealth > 0)
        {
            foreach (MeshRenderer Mesh in EnemyMeshArray)
            {
                Mesh.material.color = Color.white;
            }
        }
        else
        {
            foreach (MeshRenderer Mesh in EnemyMeshArray)
            {
                Mesh.material.color = Color.grey;
            }

            // 충돌X
            // 레이어 변경 > EnemyDead
            gameObject.layer = 12;
            // 사망처리
            IsEnemyDead = true; 
            // 추적 종료
            IsTracking = false;
            EnemyNavMeshAgent.enabled = false;

            // 애니메이션
            EnemyAnimator.SetTrigger("TriggerDie");

            // 수류탄일 경우
            if(IsGrenade )
            {
                // 넉백 처리
                ReactVector = ReactVector.normalized;
                ReactVector += Vector3.up * 3; // TODO : y축 수정해야함

                EnemyRigid.freezeRotation = false;
                EnemyRigid.AddForce(ReactVector * 5, ForceMode.Impulse);
                EnemyRigid.AddTorque(ReactVector * 15, ForceMode.Impulse);
            }
            else
            {
                // 넉백 처리
                ReactVector = ReactVector.normalized;
                ReactVector += Vector3.up;
                
                EnemyRigid.AddForce(ReactVector * 5, ForceMode.Impulse);
            }

            Destroy(gameObject, 4);
        }
    }

    /** 추적을 시작한다 */
    private void TrackingStart()
    {
        IsTracking = true;
        EnemyAnimator.SetBool("IsWalk", true);
    }

    /** 물리속도, 회전을 0으로 한다 */
    private void FreezeVelocity()
    {
        // 추적상태일 경우
        if(IsTracking)
        {
            EnemyRigid.velocity = Vector3.zero;
            EnemyRigid.angularVelocity = Vector3.zero;
        }   
    }
    #endregion // 함수
}