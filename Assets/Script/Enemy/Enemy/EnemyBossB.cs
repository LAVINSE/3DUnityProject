using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossB : Enemy
{
    #region 변수
    [SerializeField] private bool IsLook;
    [SerializeField] private float TargetRadius;
    [SerializeField] private float SlowPercent;
    [SerializeField] private GameObject PoisonPrefab;

    private Vector3 LookVector; // 바라보는 방향 예측
    #endregion // 변수

    #region 함수
    /** 초기화 */
    protected override void Awake()
    {
        base.Awake();

        IsLook = true;
    }

    /** 초기화 => 상태를 갱신한다 */
    protected override void Update()
    {
        base.Update();

        // 바라보기가 true일때
        if (IsLook)
        {
            float Horizontal = Input.GetAxisRaw("Horizontal");
            float Vertical = Input.GetAxisRaw("Vertical");
            LookVector = new Vector3(Horizontal, 0, Vertical) * 5f;
            transform.LookAt(PlayerTarget.position + LookVector);
        }
    }

    /** 타겟한다 */
    public override void Targeting()
    {
        base.Targeting();
        // 구체모양의 레이캐스트 (모든오브젝트)
        RaycastHit[] RayHitArray = Physics.SphereCastAll(this.transform.position, TargetRadius,
            transform.forward, oAttackRange, TargetLayer);

        // 플레이어가 있을 경우, 공격중 X
        if (RayHitArray.Length > 0 && !IsAttack)
        {
            StartCoroutine(Select());
        }

    }

    private IEnumerator Select()
    {
        IsAttack = true;
        yield return new WaitForSeconds(0.1f);
        IsTracking = false;

        int RandomAction = Random.Range(0, 1);

        switch (RandomAction)
        {
            case 0:
                StartCoroutine(Poison());
                break;
        }
    }

    private IEnumerator Poison()
    {
        float Run;
        float Walk;

        EnemyAnimator.SetTrigger("TriggerBigShot");
        yield return new WaitForSeconds(0.5f);

        var Player = PlayerTarget.GetComponent<PlayerAction>();

        Run = Player.PlayerRunSpeed;
        Walk = Player.PlayerWalkSpeed;

        Player.PlayerRunSpeed *= SlowPercent;
        Player.PlayerWalkSpeed *= SlowPercent;

        Vector3 Pos = new Vector3(PlayerTarget.transform.position.x, 55, PlayerTarget.transform.position.z);
        var PoisonObject = Instantiate(PoisonPrefab, Pos, Quaternion.identity);
        yield return new WaitForSeconds(1.5f);
        Player.PlayerRunSpeed = Run;
        Player.PlayerWalkSpeed = Walk;

        yield return new WaitForSeconds(4f);

        IsTracking = true;
        IsAttack = false;
    }
    #endregion // 함수
}
