using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class EnemyBossBreath : MonoBehaviour
{
    #region 변수
    [SerializeField] private int Damage;

    private PlayerAction Player;
    #endregion // 변수

    #region 프로퍼티
    public bool IsStart { get; set; }
    #endregion // 프로퍼티

    #region 함수
    private void OnDestroy()
    {
        IsStart = false;
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player = other.GetComponent<PlayerAction>();
            IsStart = true;
            StopCoroutine(Breath(Player));
            StartCoroutine(Breath(Player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IsStart = false;
        }
    }

    private IEnumerator Breath(PlayerAction Player)
    {
        while (IsStart)
        {
            Player.oHealth -= Damage;
            UIManager.Instance.PlayerStatusTextUpdate();
            Player.ChangeColor(Color.red);
            yield return new WaitForSeconds(0.2f);
            Player.ChangeColor(Color.white);
            yield return new WaitForSeconds(0.2f);
        }
    }
    #endregion // 함수
}
