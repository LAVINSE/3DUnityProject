using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : CSingleton<GameManager>
{
    #region 변수
    private bool IsEscDown;
    private bool IsCursor = true;
    #endregion // 변수

    #region 프로퍼티
    public ObjectPoolManager oPoolManager { get; private set; }
    #endregion // 프로퍼티

    #region 함수
    /** 초기화 */
    public override void Awake()
    {
        base.Awake();
        oPoolManager = CFactory.CreateObject<ObjectPoolManager>("ObjectPoolManager", this.gameObject,
            Vector3.zero, Vector3.one, Vector3.zero);
        CursorLock();
    }

    /** 초기화 => 상태를 갱신한다 */
    private void Update()
    {
        IsEscDown = Input.GetKeyDown(KeyCode.Escape);

        // 커서잠금이 활성화 상태가 아닐경우
        if(IsEscDown && !IsCursor)
        {
            CursorLock();
            IsCursor = true;

        }
        // 커서잠금이 활성화 상태일 경우
        else if (IsEscDown && IsCursor)
        {
            CursorUnLock();
            IsCursor = false;
        }
    }

    // 커서 잠금
    public void CursorLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 커서 잠금해제
    public void CursorUnLock()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    #endregion // 함수
}
