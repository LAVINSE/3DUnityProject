using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    #region 변수
    [SerializeField] MainSceneManager MainScene;
    #endregion // 변수

    #region 함수
    /** 초기화 => 접촉했을경우 (트리거) */
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 일 경우
        if (other.gameObject.CompareTag("Player"))
        {
            // 스테이지 시작
            MainScene.StageStart();
        }
    }
    #endregion // 함수
}
