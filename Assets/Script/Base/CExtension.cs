using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CExtension
{
    #region 함수
    /** 작음 여부를 검사한다 */
    public static bool ExIsLess(this float a_fSender, float a_fRhs)
    {
        return a_fSender < a_fRhs - float.Epsilon;
    }

    /** 작거나 같음 여부를 검사한다 */
    public static bool ExIsLessEquals(this float a_fSender, float a_fRhs)
    {
        return a_fSender.ExIsLess(a_fRhs) || a_fSender.ExIsEquals(a_fRhs);
    }

    /** 큰 여부를 검사한다 */
    public static bool ExIsGreat(this float a_fSender, float a_fRhs)
    {
        return a_fSender > a_fRhs + float.Epsilon;
    }

    /** 크거나 같음 여부를 검사한다 */
    public static bool ExIsGreatEquals(this float a_fSender, float a_fRhs)
    {
        return a_fSender.ExIsGreat(a_fRhs) || a_fSender.ExIsEquals(a_fRhs);
    }

    /** 같음 여부를 검사한다 */
    public static bool ExIsEquals(this float a_fSender, float a_fRhs)
    {
        return Mathf.Approximately(a_fSender, a_fRhs);
    }
    #endregion // 함수
}
