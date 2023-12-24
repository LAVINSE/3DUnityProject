using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Normal,
        Boss,
    }

    #region 변수
    [Header("=====> 적 정보 <=====")]
    [SerializeField] private ItemDropTable DropTable;

    [Space]
    [SerializeField] private EnemyType Type;
    [SerializeField] private int MaxHealth;
    [SerializeField] private int CurrentHealth;
    [SerializeField] private float TrackingRange;
    [SerializeField] private float AttackRange;
    [SerializeField] private GameObject[] CoinArray;

    [Space]
    [Header("=====> 적 공통 <=====")]
    [SerializeField] protected LayerMask TargetLayer;
    [SerializeField] protected Transform PlayerTarget;
    [SerializeField] protected bool IsAttack;
    
    protected Rigidbody EnemyRigid;
    protected BoxCollider EnemyBoxCollider;
    protected MeshRenderer[] EnemyMeshArray;  
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
    public Transform oSpawnPos { get; set; }
    public EnemyState EnemyStateMachine { get; set; }
    public NavMeshAgent oEnemyNavMeshAgent { get; set; }
    public MainSceneManager oMainSceneManager { get; set; }
    #endregion // 프로퍼티

    #region 함수
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(this.transform.position + Vector3.up *1f, this.transform.forward * AttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(this.transform.position + Vector3.up *2f, this.transform.forward * TrackingRange);

        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(this.transform.position, 1.5f);
    }

    /** 초기화 */
    protected virtual void Awake()
    {
        EnemyRigid = GetComponent<Rigidbody>();
        EnemyBoxCollider = GetComponent<BoxCollider>();
        EnemyMeshArray = GetComponentsInChildren<MeshRenderer>();
        oEnemyNavMeshAgent = GetComponent<NavMeshAgent>();
        EnemyAnimator = GetComponentInChildren<Animator>();
        EnemyStateMachine = GetComponent<EnemyState>();

        oMainSceneManager = CSceneManager.GetSceneManager<MainSceneManager>(CDefine.MainGameScene);

        CurrentHealth = MaxHealth;

        IsTracking = true;
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

            if (!IsEnemyDead)
            {
                // 체력감소
                CurrentHealth -= WeaponComponent.oWeaponMeleeDamage;
                // 넉백계산
                Vector3 ReactVec = transform.position - other.transform.position;

                StartCoroutine(OnHit(ReactVec, false));
                Debug.Log($"근접공격 체력 : {CurrentHealth}");
            } 
        }
        else if (other.gameObject.CompareTag("Bullet"))
        {
            Bullet BulletComponent = other.gameObject.GetComponent<Bullet>();

            if (!IsEnemyDead)
            {
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

        UIManager.Instance.BossHealthBarUpdate();

        // 살아있을 경우
        if(CurrentHealth > 0)
        {
            yield return new WaitForSeconds(0.1f);
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
            oEnemyNavMeshAgent.enabled = false;

            // 애니메이션
            EnemyAnimator.SetTrigger("TriggerDie");

            // 드랍 아이템
            EnemyDropItem();

            switch (Type)
            {
                case EnemyType.Normal:
                    oMainSceneManager.oEnemyCount--;
                    break;
                case EnemyType.Boss:
                    oMainSceneManager.oEnemyBossCount--;
                    break;
            }

            UIManager.Instance.EnemyCountTextUpdate();

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
        EnemyAnimator.SetBool("IsWalk", true);
    }

    /** 추적중 */
    public void Tracking(Transform TargetPos)
    {
        this.TargetPos = TargetPos;

        // 추적상태일 경우
        if (oEnemyNavMeshAgent.enabled)
        {
            // 도착할 목표 지정
            oEnemyNavMeshAgent.SetDestination(this.TargetPos.position);

            // 추적중일 경우 >> 추적, 아닐경우 >> 멈춤
            oEnemyNavMeshAgent.isStopped = !IsTracking;
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

    private void EnemyDropItem()
    {
        var DropItem = DropTable.ItemDrop();

        if (DropItem != null)
        {
            for (int i = 0; i < DropItem.Length; i++)
            {
                Instantiate(DropItem[i].ItemPrefab, this.transform.position + Vector3.up * 2f, quaternion.identity);
            }
        }
    }
    #endregion // 함수
}