using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Timeline.Actions.MenuPriority;
using static UnityEngine.Rendering.VolumeComponent;

public partial class PlayerAction : MonoBehaviour
{
    #region 변수
    [Header("=====> 플레이어 속도 <=====")]
    [SerializeField] private float PlayerWalkSpeed = 0f;
    [SerializeField] private float PlayerRunSpeed = 0f;
    [SerializeField] private float PlayerJumpPower = 0f;

    [Space]
    [Header("=====> 플레이어 쿨타임 <=====")]
    [SerializeField] private float PlayerDodgeCoolTime = 0f;
    [SerializeField] private float PlayerWeaponSwapCoolTime = 0f;

    [Space]
    [Header("=====> 플레이어 회피 설정 <=====")]
    [Tooltip(" 회피속도 배율 ")]
    [SerializeField] private float PlayerDodgeMagnification = 0f;
    [Tooltip(" 회피속도 원상복구까지 걸리는 시간")]
    [SerializeField] private float PlayerDodgeSpeedRestoreTime = 0f;

    [Space]
    [Header("=====> 아이템 현재 가지가고 있는 수 <=====")]
    [SerializeField] private int Ammo;
    [SerializeField] private int Coin;
    [SerializeField] private int Health;
    [SerializeField] private int HasGrenades;

    [Space]
    [Header("=====> 아이템 최대치 <=====")]
    [SerializeField] private int MaxAmmo;
    [SerializeField] private int MaxCoin;
    [SerializeField] private int MaxHealth;
    [SerializeField] private int MaxHasGrenades;

    [Space]
    [Header("=====> 플레이어 장착가능한장비 or 아이템 <=====")]
    [SerializeField] private GameObject[] HandWeaponArray;
    [SerializeField] private GameObject GrenadePrefab;
    [SerializeField] private bool[] HasWeaponArray;


    [Space]
    [Header("=====> 플레이어 설정 <=====")]
    public Transform CameraArm;
    public Inventory Inven;

    // 장착무기번호
    private int EquipWeaponIndex = -1;

    // 이동정보
    private float HorizonAxis;
    private float VerticalAxis;

    // 공격 딜레이
    private float AttackDelay;
    
    // 입력정보
    private bool IsRunDown;
    private bool IsJumpDown;
    private bool IsDodgeDown;
    private bool IsInteractionDown;
    private bool IsAddItemDown; 
    private bool IsAttackDown;
    private bool IsReloadDown;
    private bool IsGrenadeDown;
    private bool IsSwapWeapon_0;
    private bool IsSwapWeapon_1;
    private bool IsSwapWeapon_2;
    private bool IsInventoryDown;

    // 동작중인지 확인
    private bool IsAttackReady = true;
    private bool IsJump;
    private bool IsDodge;
    private bool IsSwap;
    private bool IsReloadReady;
    private bool IsDamage;
    private bool IsDead;

    // 쿨타임
    private bool IsDodgeCoolTime;

    private Animator PlayerAnimator;
    private MeshRenderer[] PlayerMeshRenderArray;
    private Rigidbody PlayerRigid;

    // CallBack
    private event Action DodgeCallback;
    private event Action SwapCallback;
    #endregion // 변수

    #region 프로퍼티 
    public int oCoin
    {
        get => Coin;
        set => Coin = value;
    }
    public int oScroe { get; set; }
    public int oHealth
    {
        get => Health;
        set => Health = value;
    }
    public int oAmmo
    {
        get => Ammo;
        set => Ammo = value;
    }
    public int oHasGrenades
    {
        get => HasGrenades;
        set => HasGrenades = value;
    }
    public int oMaxHealth
    {
        get => MaxHealth;
        set => MaxHealth = value;
    }
    public int oMaxCoin
    {
        get => MaxCoin;
        set => MaxCoin = value;
    }
    public int oMaxAmmo
    {
        get => MaxAmmo;
        set => MaxAmmo = value;
    }
    public int oMaxHasGrenades
    {
        get => MaxHasGrenades;
        set => MaxHasGrenades = value;
    }

    // 행동중 확인
    public bool IsShop { get; set; }

    // 장착중인 무기
    public Weapon oEquipWeapon { get; set; }
    public bool[] oHasWeaponArray
    {
        get => HasWeaponArray;
        set => HasWeaponArray = value;
    }

    // 아이템 줍기
    public Item oItem { get; set; }
    // 상호작용
    public GameObject oNearObject { get; set; }

    public MainSceneManager oMainSceneManager;
    #endregion // 프로퍼티 

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        PlayerRigid = GetComponent<Rigidbody>();
        PlayerAnimator = GetComponentInChildren<Animator>();
        PlayerMeshRenderArray = GetComponentsInChildren<MeshRenderer>();

        // Callback 등록
        DodgeCallback += CoolTimePlayerDodge;
        SwapCallback += CoolTimePlayerSwap;

        // 점수 저장
        PlayerPrefs.SetInt("MaxScore", 112500);
    }

    /** 초기화 => 상태를 갱신한다 */
    private void Update()
    {
        // 플레이어 입력처리
        PlayerInput();
        // 플레이어 이동
        PlayerMove();
        // 플레이어 점프
        PlayerJump();
        // 플레이어 회피
        PlayerDodge();
        // 플레이어 상호작용
        PlayerInteraction();
        // 플레이어 인벤토리
        PlayerInventory();
        // 플레이어 무기교체
        PlayerWeaponSwap();
        // 플레이어 아이템 줍기
        PlayerAddItem();
        // 플레이어 공격
        PlayerAttack();
        // 플레이어 장전
        PlayerReload();
        // 플레이어 수류탄 던지기
        PlayerThrowGrenade();
    }
    /** 초기화 => 접촉했을 경우 (트리거) */
    private void OnTriggerEnter(Collider other)
    {
        // 적 총알에 맞았을 경우
        if (other.gameObject.CompareTag("EnemyAttack"))
        {
            // 데미지를 받지 않는중일경우
            if (!IsDamage)
            {
                EnemyAttack EnemyAttackComponent = other.GetComponent<EnemyAttack>();

                // 적 총알데미지 만큼 체력감소
                Health -= EnemyAttackComponent.oAttackDamage;
                Debug.Log($"적 공격 체력 : {Health}");

                bool IsBossTaunt = other.gameObject.name == "MeleeTauntArea";

                // 1초 무적판정
                StartCoroutine(OnHit(IsBossTaunt));
            }

            // 근접공격은 RigidBody가 없다, 원거리공격만 Null이 아니다
            if (other.GetComponent<Rigidbody>() != null)
            {
                // 원거리 공격 삭제
                Destroy(other.gameObject);
            }
        }
    }

    /** 플레이어 입력처리 */
    private void PlayerInput()
    {
        // 상하좌우 이동
        HorizonAxis = Input.GetAxisRaw("Horizontal");
        VerticalAxis = Input.GetAxisRaw("Vertical");

        // 달리기 shift
        IsRunDown = Input.GetButton("Run");
        // 점프
        IsJumpDown = Input.GetKeyDown(KeyCode.Space);
        // 회피
        IsDodgeDown = Input.GetKeyDown(KeyCode.C);
        // 상호작용
        IsInteractionDown = Input.GetKeyDown(KeyCode.F);
        // 아이템 줍기
        IsAddItemDown = Input.GetKeyDown(KeyCode.Z);
        // 공격
        IsAttackDown = Input.GetKey(KeyCode.Mouse0);
        // 재장전
        IsReloadDown = Input.GetKeyDown(KeyCode.R);
        // 수류탄
        IsGrenadeDown = Input.GetKeyDown(KeyCode.Mouse1);
        // 인벤토리
        IsInventoryDown = Input.GetKeyDown(KeyCode.I);


        // 무기 교체 단축키
        IsSwapWeapon_0 = Input.GetKeyDown(KeyCode.Alpha1);
        IsSwapWeapon_1 = Input.GetKeyDown(KeyCode.Alpha2);
        IsSwapWeapon_2 = Input.GetKeyDown(KeyCode.Alpha3);
    }

    /** 플레이어가 회피한다 */
    private void PlayerDodge()
    {
        // 회피를 눌렀을 경우, 점프중X, 회피중X, 무기교체중X, 재장전X
        if (IsDodgeDown && !IsJump && !IsDodge && !IsSwap && !IsDodgeCoolTime && !IsReloadReady && !IsShop && !IsDead)
        {
            // 속도 변화
            PlayerWalkSpeed *= PlayerDodgeMagnification;
            PlayerRunSpeed *= PlayerDodgeMagnification;

            // 애니메이션
            PlayerAnimator.SetTrigger("TriggerDodge");
            IsDodge = true;
            IsDodgeCoolTime = true;


            // 속도 원상복구
            Invoke("DonePlayerDodge", PlayerDodgeSpeedRestoreTime);

            // 회피 쿨타임 적용
            StartCoroutine(SKillCoolDown(PlayerDodgeCoolTime, DodgeCallback));
        }
    }

    /** 플레이어 회피가 끝났을때 */
    private void DonePlayerDodge()
    {
        PlayerWalkSpeed = PlayerWalkSpeed / PlayerDodgeMagnification;
        PlayerRunSpeed = PlayerRunSpeed / PlayerDodgeMagnification;
        IsDodge = false;
    }

    /** 플레이어 회피 쿨타임이 끝나면 회피 가능 */
    private void CoolTimePlayerDodge()
    {
        Debug.Log("회피가능");
        IsDodgeCoolTime = false;
    }

    /** 플레이어 상호작용 */
    private void PlayerInteraction()
    {
        // 상호작용 키를 눌렀을 경우, 점프X, 회피X
        if (IsInteractionDown && oNearObject != null && !IsJump && !IsDodge && !IsDead)
        {
            // 상호작용 오브젝트 태그
            switch (oNearObject.tag)
            {
                case "Shop":
                    Shop NpcShop = oNearObject.GetComponent<Shop>();

                    // 상점 입장
                    NpcShop.Enter();
                    IsShop = true;
                    break;
            }
        }
    }

    /** 플레이어 인벤토리를 활성/비활성화 한다 */
    private void PlayerInventory()
    {
        if(IsInventoryDown)
        {
            if(Inven.gameObject.activeSelf)
            {
                Inven.CloseInventory();
            }
            else
            {
                Inven.OpenInventory();
            } 
        }
    }

    /** 플레이어 무기 교체 */
    private void PlayerWeaponSwap()
    {
        // 장착하고 있는 아이템이 없을 경우
        if (IsSwapWeapon_0 && (!HasWeaponArray[0] || EquipWeaponIndex == 0)) return; // 1번
        if (IsSwapWeapon_1 && (!HasWeaponArray[1] || EquipWeaponIndex == 1)) return; // 2번
        if (IsSwapWeapon_2 && (!HasWeaponArray[2] || EquipWeaponIndex == 2)) return; // 3번

        int WeaponIndex = -1;
        
        if(IsSwapWeapon_0) { WeaponIndex = 0; } // 1번
        if(IsSwapWeapon_1) { WeaponIndex = 1; } // 2번
        if(IsSwapWeapon_2) { WeaponIndex = 2; } // 3번

        // 교체키를 눌렀을 경우, 점프중X, 회피중X, 무기교체중X
        if((IsSwapWeapon_0 || IsSwapWeapon_1 || IsSwapWeapon_2) && !IsJump & !IsDodge && !IsSwap && !IsShop)
        {
            // 장착한 무기가 있을 경우
            if(oEquipWeapon != null)
            {
                oEquipWeapon.gameObject.SetActive(false);
            }

            // 단축키 번호에 맞게 무기 활성화
            EquipWeaponIndex = WeaponIndex;
            oEquipWeapon = HandWeaponArray[WeaponIndex].GetComponent<Weapon>();
            oEquipWeapon.gameObject.SetActive(true);

            // 애니메이션
            PlayerAnimator.SetTrigger("TriggerSwap");

            IsSwap = true;

            // 무기 교체하는동안 쿨타임
            StartCoroutine(SKillCoolDown(PlayerWeaponSwapCoolTime, SwapCallback));
        }
    }

    /** 플레이어 무기교체 쿨타임이 끝나면 행동가능 (움직이는건 가능) */
    private void CoolTimePlayerSwap()
    {
        Debug.Log("무기교체완료");
        IsSwap = false;
    }

    /** 플레이어 아이템 줍기*/
    private void PlayerAddItem()
    {
        // Z키를 눌렀을 경우
        if (IsAddItemDown)
        {
            // 아이템이 없을 경우
            if (oItem == null)
            {
                Debug.Log("아이템이 없습니다");
                return;
            }

            // TODO : 아이템 사용 함수 만들기
            // 아이템 타입에따라 행동
            switch (oItem.oType)
            {
                case Item.ItemType.Ammo:
                    Ammo += oItem.oItemValue;
                    if (Ammo > MaxAmmo)
                    {
                        Inven.AcquireItem(oItem.ItemDataTable, oItem.oItemValue);
                        Ammo = MaxAmmo;
                    }
                    break;
                case Item.ItemType.Coin:
                    Coin += oItem.oItemValue;
                    if (Coin > MaxCoin)
                    {
                        Coin = MaxCoin;
                    }
                    break;
                case Item.ItemType.Heart:
                    Health += oItem.oItemValue;
                    if (Health > MaxHealth)
                    {
                        Health = MaxHealth;
                    }
                    break;
                case Item.ItemType.Grenade:
                    HasGrenades += oItem.oItemValue;
                    if (HasGrenades > MaxHasGrenades)
                    {
                        HasGrenades = MaxHasGrenades;
                    }
                    break;
                case Item.ItemType.Weapon:
                    // 무기 인덱스 번호를 가져온다
                    int WeaponIndex = oItem.oWeaponIndex;
                    HasWeaponArray[WeaponIndex] = true;

                    Destroy(oNearObject);
                    break;
            }

            // 아이템 파괴
            Destroy(oItem.gameObject);
        }
    }

    /** 플레이어 공격 */
    private void PlayerAttack()
    {
        if (oEquipWeapon == null) { return; }

        // 공격 딜레이 적용
        AttackDelay += Time.deltaTime;

        // 공격 딜레이가 공격속도보다 클 경우, 공격가능
        IsAttackReady = oEquipWeapon.oWeaponRate < AttackDelay;

        // 마우스 왼쪽클릭일 경우, 공격준비가능, 회피X, 무기교체X, 재장전X
        if(IsAttackDown && IsAttackReady && !IsDodge && !IsSwap && !IsReloadReady && !IsShop && !IsDead)
        {
            // 무기 사용
            oEquipWeapon.WeaponUse();

            // 애니메이션
            if(oEquipWeapon.oType == Weapon.WeaponType.Melee)
            {
                PlayerAnimator.SetTrigger("TriggerSwing");
            }
            else if(oEquipWeapon.oType == Weapon.WeaponType.Range && oEquipWeapon.oCurrentAmmo != 0)
            {
                PlayerAnimator.SetTrigger("TriggerShot");
            }
            
            // 공격 딜레이 초기화
            AttackDelay = 0;
        }
    }

    /** 플레이어 재장전 */
    private void PlayerReload()
    {
        // 무기가없거나, 근접무기거나, 총알이 없으면 실행X
        if(oEquipWeapon == null) { return; }
        if(oEquipWeapon.oType == Weapon.WeaponType.Melee) { return; }
        if(Ammo == 0) { return; }

        // 리로드키를 눌렀을경우, 회피X, 무기교체X, 점프X, 공격가능한상태일때, 재장전중이 아닐때
        if(IsReloadDown && !IsJump && !IsDodge && !IsSwap && IsAttackReady && !IsReloadReady && !IsShop && !IsDead)
        {
            Debug.Log("리로드");
            PlayerAnimator.SetTrigger("TriggerReload");
            IsReloadReady = true;

            Invoke("DonePlayerReload", 3f);
        }
    }

    /** 플레이어 재장전이 끝났을때 */
    private void DonePlayerReload()
    {
        // 총알 장전
        int ReloadAmmo = Ammo + oEquipWeapon.oCurrentAmmo < oEquipWeapon.oMaxAmmo ?
            Ammo : oEquipWeapon.oMaxAmmo - oEquipWeapon.oCurrentAmmo;

        oEquipWeapon.oCurrentAmmo += ReloadAmmo;
        Ammo -= ReloadAmmo;

        // 장전 완료
        IsReloadReady = false;
    }

    // 플레이어 수류탄 던지기
    private void PlayerThrowGrenade()
    {
        // 수류탄이 없을경우
        if(HasGrenades == 0) { return; }

        // 수류탄키를 눌렀을 경우, 재장전X, 무기교체X, 회피X
        if(IsGrenadeDown && !IsReloadReady && !IsSwap && !IsDodge && !IsShop & !IsDead)
        {
            // 수류탄 생성
            GameObject GrenadeObj = Instantiate(GrenadePrefab, transform.position, transform.rotation);
            Rigidbody RigidGrenade = GrenadeObj.GetComponent<Rigidbody>();
            // TODO : 던지는 방향 살짝 수정필요
            // 수류탄 던지기, 회전주기
            RigidGrenade.AddForce(this.transform.forward * 10, ForceMode.Impulse);
            RigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

            // 가지고 있는 수류탄개수 하나 제거
            HasGrenades--;
        }
    }

    /** 플레이어 피격 */
    private IEnumerator OnHit(bool IsBossTaunt)
    {
        IsDamage = true;

        // 피격 색상
        foreach(MeshRenderer Mesh in PlayerMeshRenderArray)
        {
            Mesh.material.color = Color.yellow;
        }

        if (IsBossTaunt)
        {
            PlayerRigid.AddForce(transform.forward * -25, ForceMode.Impulse); 
        }

        if (Health <= 0 && !IsDead)
        {
            OnDie();
        }

        // 피격 후 1초 무적
        yield return new WaitForSeconds(1);

        IsDamage = false;
        // 피격 색상 복구
        foreach (MeshRenderer Mesh in PlayerMeshRenderArray)
        {
            Mesh.material.color = Color.white;
        }

        if (IsBossTaunt)
        {
            PlayerRigid.velocity = Vector3.zero;
        }
    }

    /** 플레이어 죽음 */
    private void OnDie()
    {
        PlayerAnimator.SetTrigger("TriggerDie");
        IsDead = true;
        oMainSceneManager.GameOver();
    }

    /** 쿨타임 적용 */
    private IEnumerator SKillCoolDown(float CoolTime, Action Callback, Image SkillImg = null)
    {
        float CurrentTime = 0.0f;
        CurrentTime = CoolTime;

        while(CurrentTime > 0.0f)
        {
            CurrentTime -= Time.deltaTime;

            // 스킬이미지가 있을경우
            if(SkillImg != null)
            {
                SkillImg.fillAmount = (CurrentTime / CoolTime);
            }
            
            yield return new WaitForFixedUpdate();
        }

        Callback?.Invoke();
    }
    #endregion // 함수
}
