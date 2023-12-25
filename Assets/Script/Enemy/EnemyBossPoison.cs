using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossPoison : MonoBehaviour
{
    #region 변수
    [SerializeField] private int Damage;
    [SerializeField] private float Timer = 0;

    private PlayerAction Player;
    private bool IsExit;
    #endregion // 변수

    #region 함수
    private void Update()
    {
        Timer -= Time.deltaTime;

        if(Timer <= 0)
        {
            IsExit = false;
            StopAllCoroutines();
            UIManager.Instance.PlayerStatusTextUpdate();
            Player.ChangeColor(Color.white);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player = other.GetComponent<PlayerAction>();
            IsExit = true;
            StopCoroutine(Poison(Player));
            StartCoroutine(Poison(Player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IsExit = false;
        }
    }

    private IEnumerator Poison(PlayerAction Player)
    {
        while (IsExit)
        {
            Player.oHealth -= Damage;
            UIManager.Instance.PlayerStatusTextUpdate();
            Player.ChangeColor(Color.red);
            yield return new WaitForSeconds(0.5f);
            Player.ChangeColor(Color.white);
            yield return new WaitForSeconds(0.5f);
        }
    }
    #endregion // 함수
}
