using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBossBurn : MonoBehaviour
{
    #region 변수
    [SerializeField] private int Damage;

    private PlayerAction Player;
    private bool IsStart;
    #endregion // 변수

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
            StopCoroutine(Burn(Player));
            StartCoroutine(Burn(Player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IsStart = false;
        }
    }

    private IEnumerator Burn(PlayerAction Player)
    {
        while (IsStart)
        {
            Player.oHealth -= Damage;
            UIManager.Instance.PlayerStatusTextUpdate();
            Player.ChangeColor(Color.red);
            yield return new WaitForSeconds(0.7f);
            Player.ChangeColor(Color.white);
            yield return new WaitForSeconds(0.7f);
        }
    }
    #endregion // 함수
}
