using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region 변수
    private bool IsEscDown;
    private bool IsCursor = true;
    #endregion // 변수

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        CursorOn();
    }

    /** 초기화 => 상태를 갱신한다 */
    private void Update()
    {
        IsEscDown = Input.GetKeyDown(KeyCode.Escape);

        // 커서잠금이 활성화 상태가 아닐경우
        if(IsEscDown && !IsCursor)
        {
            CursorOn();
            IsCursor = true;

        }
        // 커서잠금이 활성화 상태일 경우
        else if (IsEscDown && IsCursor)
        {
            CursorOff();
            IsCursor = false;
        }
    }

    // 커서 잠금
    private void CursorOn()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 커서 잠금해제
    private void CursorOff()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    #endregion // 함수
}
