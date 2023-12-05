using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    #region 함수
    /** 초기화 => 상태를 갱신한다 */
    private void Update()
    {
        this.transform.Rotate(Vector3.right * 30 * Time.deltaTime);
    }
    #endregion // 함수
}
