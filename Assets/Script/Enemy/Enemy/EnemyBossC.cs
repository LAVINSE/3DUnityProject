using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossC : Enemy
{
    #region 변수
    [SerializeField] private bool IsLook;
    [SerializeField] private float TargetRadius;
    [SerializeField] private GameObject BreathPrefab;
    [SerializeField] private GameObject BreathRoot;
    [SerializeField] private GameObject BurnPrefab;

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

    // 패턴을 선택한다
    private IEnumerator Select()
    {
        IsAttack = true;
        yield return new WaitForSeconds(0.1f);
        IsTracking = false;

        int RandomAction = Random.Range(0, 2);

        switch (RandomAction)
        {
            case 0:
                StartCoroutine(Burn());
                break;
            case 1:
                StartCoroutine(Breath());
                break;
        }
    }

    private IEnumerator Breath()
    {
        EnemyAnimator.SetTrigger("TriggerShot");
        yield return new WaitForSeconds(1f);
        var BreathObject = Instantiate(BreathPrefab, BreathRoot.transform);
        yield return new WaitForSeconds(3f);
        Destroy(BreathObject);
        UIManager.Instance.PlayerStatusTextUpdate();
        PlayerTarget.GetComponent<PlayerAction>().ChangeColor(Color.white);
        yield return new WaitForSeconds(3f);

        IsTracking = true;
        IsAttack = false;
    }

    private IEnumerator Burn()
    {
        EnemyAnimator.SetTrigger("TriggerBigShot");
        yield return new WaitForSeconds(1f);
        var BurnObject = Instantiate(BurnPrefab, this.transform);
        yield return new WaitForSeconds(5f);
        Destroy(BurnObject);
        UIManager.Instance.PlayerStatusTextUpdate();
        PlayerTarget.GetComponent<PlayerAction>().ChangeColor(Color.white);
        yield return new WaitForSeconds(2f);

        IsTracking = true;
        IsAttack = false;
    }
    #endregion // 함수
}
