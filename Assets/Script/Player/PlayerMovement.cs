using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region 변수
    [SerializeField] private float PlayerWalkSpeed = 0f;
    [SerializeField] private float PlayerRunSpeed = 0f;
    [SerializeField] private float PlayerJumpPower = 0f;
    [Tooltip(" 회피속도 배율 ")]
    [SerializeField] private float PlayerDodgeMagnification = 0f;
    [Tooltip(" 회피속도 원상복구까지 걸리는 시간")]
    [SerializeField] private float PlayerDodgeSpeedRestoreTime = 0f;
    [SerializeField] private float PlayerDodgeCoolTime = 0f;

    private float HorizonAxis;
    private float VerticalAxis;
    private bool IsRunDown;
    private bool IsJumpDown;
    private bool IsDodgeDown;
    private bool IsJump;
    private bool IsDodge;

    private Vector3 MoveVector;
    private Vector3 DodgeVector;
    private Rigidbody PlayerRigid;
    private Animator PlayerAnimator;

    private event Action DodgeCallback;
    #endregion // 변수

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        PlayerRigid = GetComponent<Rigidbody>();
        PlayerAnimator = GetComponentInChildren<Animator>();

        // Callback 등록
        DodgeCallback += CoolTimePlayerDodge;
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
    }

    /** 초기화 => 접촉했을경우 */
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            // 플레이어 점프 후 착지했을 때
            DonePlayerJump();
        }
    }

    /** 플레이어 입력처리 */
    private void PlayerInput()
    {
        HorizonAxis = Input.GetAxisRaw("Horizontal");
        VerticalAxis = Input.GetAxisRaw("Vertical");
        IsRunDown = Input.GetButton("Run");
        IsJumpDown = Input.GetKeyDown(KeyCode.Space);
        IsDodgeDown = Input.GetKeyDown(KeyCode.C);
    }

    /** 플레이어가 움직인다 */
    private void PlayerMove()
    {
        MoveVector = new Vector3(HorizonAxis, 0, VerticalAxis).normalized;

        // 달리기를 눌렀을 경우
        if(IsRunDown == true)
        {
            MoveVector *= PlayerRunSpeed * Time.deltaTime;
        }
        else
        {
            MoveVector *= PlayerWalkSpeed * Time.deltaTime;
        }   

        // 플레이어 움직임
        this.transform.position += MoveVector;

        // 애니메이션
        PlayerAnimator.SetBool("IsWalk", MoveVector != Vector3.zero);
        PlayerAnimator.SetBool("IsRun", IsRunDown);

        // 플레이어 방향바라보게 설정
        transform.LookAt(this.transform.position + MoveVector);
    }

    /** 플레이어가 점프한다 */
    private void PlayerJump()
    {
        // 점프를 눌렀을 경우
        if(IsJumpDown == true && IsJump == false && IsDodge == false)
        {
            PlayerRigid.AddForce(Vector3.up * PlayerJumpPower, ForceMode.Impulse);
            
            // 애니메이션
            PlayerAnimator.SetBool("IsJump", true);
            PlayerAnimator.SetTrigger("TriggerJump");

            IsJump = true;
        }
    }

    /** 플레이어가 착지했을 때 */
    private void DonePlayerJump()
    {
        PlayerAnimator.SetBool("IsJump", false);
        IsJump = false;
    }

    /** 플레이어가 회피한다 */
    private void PlayerDodge()
    {
        // 회피를 눌렀을 경우
        if (IsDodgeDown == true && IsJumpDown == false && IsJump == false && IsDodge == false)
        {
            // 속도 변화
            PlayerWalkSpeed *= PlayerDodgeMagnification;
            PlayerRunSpeed *= PlayerDodgeMagnification;

            // 애니메이션
            PlayerAnimator.SetTrigger("TriggerDodge");
            IsDodge = true;

            // 속도 원상복구
            Invoke("DonePlayerDodge", PlayerDodgeSpeedRestoreTime);

            // 스킬 쿨타임 적용
            StartCoroutine(SKillCoolDown(PlayerDodgeCoolTime, DodgeCallback));
        }
    }

    /** 플레이어 회피가 끝났을때 */
    private void DonePlayerDodge()
    {
        PlayerWalkSpeed = PlayerWalkSpeed / PlayerDodgeMagnification;
        PlayerRunSpeed = PlayerRunSpeed / PlayerDodgeMagnification;
    }

    /** 플레이어 회피 쿨타임이 끝나면 회피 가능 */
    private void CoolTimePlayerDodge()
    {
        Debug.Log("회피가능");
        IsDodge = false;
    }

    /** 쿨타임 적용 */
    private IEnumerator SKillCoolDown(float CoolTime, Action Callback)
    {
        float CurrentTime = 0.0f;

        while(CurrentTime < CoolTime)
        {
            CurrentTime += Time.deltaTime;
            yield return null;
        }

        Callback?.Invoke();
    }
    #endregion // 함수
}
