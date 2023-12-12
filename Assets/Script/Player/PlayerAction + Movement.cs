using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerAction : MonoBehaviour
{
    #region 변수
    [SerializeField] private LayerMask Layer;
    [SerializeField] private bool IsWall;
    // 행동중 확인
    private bool IsMove;

    private Vector3 MoveVector;
    #endregion // 변수

    #region 함수
    /** 초기화 => 접촉했을 경우 */
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            PlayerAnimator.SetBool("IsJump", false);
            IsJump = false;
        }
    }
    private void FixedUpdate()
    {
        StopToWall();
    }
    /** 플레이어가 움직인다 */
    private void PlayerMove()
    {
        Vector2 MoveInput = new Vector2(HorizonAxis, VerticalAxis).normalized;

        // 움직임 입력이 들어왔는지 확인
        IsMove = MoveInput.magnitude != 0;

        // 입력이 들어왔을경우
        if (IsMove)
        {
            // 방향
            Vector3 LookForward = new Vector3(CameraArm.forward.x, 0f, CameraArm.forward.z).normalized;
            Vector3 LookRight = new Vector3(CameraArm.right.x, 0f, CameraArm.right.z).normalized;
            // 바라보고있는 방향기준으로 이동방향 구하기
            Vector3 MoveDirect = LookForward * MoveInput.y + LookRight * MoveInput.x;

            // 이동방향 적용
            //MoveVector = MoveDirect;

            MoveVector = MoveDirect;
            // 캐릭터 방향
            this.transform.forward = MoveDirect;
        }

        // 움직임 제한
        PlayerMoveLimit();

        // 플레이어 움직임
        if (!IsWall)
        {
            transform.position += MoveVector;
        }

        // 애니메이션
        PlayerMoveAnimation();
    }

    /** 플레이어 움직임을 제어한다 */
    private void PlayerMoveLimit()
    {
        // 무기교체일경우
        if (IsSwap)
        {
            MoveVector = Vector3.zero;
        }

        // 달리기를 눌렀을 경우
        if (IsRunDown)
        {
            MoveVector *= PlayerRunSpeed * Time.deltaTime;
        }
        else
        {
            MoveVector *= PlayerWalkSpeed * Time.deltaTime;
        }
        
    }

    /** 플레이어 움직임 애니메이션 */
    private void PlayerMoveAnimation()
    {
        // 애니메이션
        PlayerAnimator.SetBool("IsWalk", IsMove);
        PlayerAnimator.SetBool("IsRun", IsRunDown && IsMove);
    }

    /** 플레이어가 점프한다 */
    private void PlayerJump()
    {
        // 점프를 눌렀을 경우, 점프중X, 회피중X, 무기교체중X, 재장전X
        if (IsJumpDown && !IsJump && !IsDodge && !IsSwap & !IsReloadReady)
        {
            PlayerRigid.AddForce(Vector3.up * PlayerJumpPower, ForceMode.Impulse);

            // 애니메이션
            PlayerAnimator.SetBool("IsJump", true);
            PlayerAnimator.SetTrigger("TriggerJump");

            IsJump = true;
        }
    }

    /** 벽 통과 방지 */
    private void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 4, Color.green);
        IsWall = Physics.Raycast(transform.position, MoveVector,
            4, Layer);
    }
    #endregion // 함수
}
