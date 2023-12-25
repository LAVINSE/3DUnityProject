using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneStatue : MonoBehaviour
{
    #region 변수
    [SerializeField] private int MaxHealth;
    [SerializeField] private int CurrentHealth;

    private MeshRenderer StoneMesh;
    public MainSceneManager oMainSceneManager { get; set; }
    #endregion // 변수

    #region 프로퍼티
    public int oMaxHealth => MaxHealth;
    public int oCurrentHealth => CurrentHealth;
    #endregion // 프로퍼티

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        CurrentHealth = MaxHealth;
        StoneMesh = GetComponent<MeshRenderer>();
        oMainSceneManager = CSceneManager.GetSceneManager<MainSceneManager>(CDefine.MainGameScene);
    }

    /** 초기화 => 접촉했을경우 (트리거) */
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyAttack"))
        {
            EnemyAttack EnemyAttackComponent = other.GetComponent<EnemyAttack>();
            CurrentHealth -= EnemyAttackComponent.oAttackDamage;
            Debug.Log($"석상 남은 체력 : {CurrentHealth}");

            StartCoroutine(OnHit());
        }
    }

    /** 석상 피격 */
    private IEnumerator OnHit()
    {
        StoneMesh.material.color = Color.red;
        UIManager.Instance.PlayerStatusTextUpdate();
        yield return new WaitForSeconds(0.1f);

        // 살아있을 경우
        if(CurrentHealth > 0)
        {
            StoneMesh.material.color = Color.white;
        }
        // 죽어있을 경우
        else
        {
            StoneMesh.material.color = Color.gray;
            gameObject.layer = 12;

            yield return new WaitForSeconds(1f);

            // 게임 종료
            oMainSceneManager.GameOver();
        }
    }
    #endregion // 함수
}
