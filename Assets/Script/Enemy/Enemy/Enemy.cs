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
    [SerializeField] private int Score;
    [SerializeField] private float TrackingRange;
    [SerializeField] private float AttackRange;
    [SerializeField] private GameObject[] CoinArray;

    [Space]
    [Header("=====> 적 공통 <=====")]
    [SerializeField] protected Transform PlayerTarget;
    [SerializeField] protected Transform StoneStatueTarget;
    [SerializeField] protected bool IsAttack;
    
    protected Rigidbody EnemyRigid;
    protected BoxCollider EnemyBoxCollider;
    protected MeshRenderer[] EnemyMeshArray;
    protected NavMeshAgent EnemyNavMeshAgent;
    protected Animator EnemyAnimator;

    protected bool IsEnemyDead;

    private Transform TargetPos;
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
    public float oTrackingRange => TrackingRange;
    public float oAttackRange => AttackRange;
    public bool IsTracking { get; set; }
    public Transform oPlayerTarget
    {
        get => PlayerTarget;
        set => PlayerTarget = value;
    }
    public Transform oStoneStatueTarget
    {
        get => StoneStatueTarget;
        set => StoneStatueTarget = value;
    }
    public EnemyState EnemyStateMachine { get; set; }
    #endregion // 프로퍼티

    #region 함수
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(this.transform.position + Vector3.up *1f, this.transform.forward * AttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(this.transform.position + Vector3.up *2f, this.transform.forward * TrackingRange);
    }

    /** 초기화 */
    protected virtual void Awake()
    {
        EnemyRigid = GetComponent<Rigidbody>();
        EnemyBoxCollider = GetComponent<BoxCollider>();
        EnemyMeshArray = GetComponentsInChildren<MeshRenderer>();
        EnemyNavMeshAgent = GetComponent<NavMeshAgent>();
        EnemyAnimator = GetComponentInChildren<Animator>();
        EnemyStateMachine = GetComponent<EnemyState>();
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

    /** 플레이어를 타겟한다 */
    public virtual void Targeting()
    {
        
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
        ChageColor(Color.red);

        yield return new WaitForSeconds(0.1f);

        // 살아있을 경우
        if(CurrentHealth > 0)
        {
            ChageColor(Color.white);
        }
        // 죽었을 경우
        else
        {
            // 색상 변경
            ChageColor(Color.grey);

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
            PlayerAction Player = PlayerTarget.GetComponent<PlayerAction>();
            Player.oScroe += Score;

            // 코인 3개중 랜덤
            int RanCoin = Random.Range(0, 3);
            // 가중치 랜덤 함수 추가예정
            Instantiate(CoinArray[RanCoin], transform.position, Quaternion.identity);

            // 카운트 감소
            //this.oMainSceneManager.oEnemyCount--;

            // 수류탄일 경우
            if(IsGrenade)
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
    public void TrackingStart()
    {
        IsTracking = true;
        EnemyAnimator.SetBool("IsWalk", true);
    }

    /** 추적중 */
    public void Tracking(Transform TargetPos)
    {
        this.TargetPos = TargetPos;
        // 추적상태일 경우
        if (EnemyNavMeshAgent.enabled && !IsAttack)
        {
            // 도착할 목표 지정
            EnemyNavMeshAgent.SetDestination(this.TargetPos.position);

            // 추적중일 경우 >> 추적, 아닐경우 >> 멈춤
            EnemyNavMeshAgent.isStopped = !IsTracking;
        }
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

    /** 색상을 변경한다 */
    private void ChageColor(Color Color)
    {
        foreach (MeshRenderer Mesh in EnemyMeshArray)
        {
            Mesh.material.color = Color;
        }
    }
    #endregion // 함수
}