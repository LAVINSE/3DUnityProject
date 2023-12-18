using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSceneManager : MonoBehaviour
{
    #region 프로퍼티
    public GameObject PublicRoot { get; private set; }

    // 아직 사용안함
    public GameObject PlayerObj { get; set; }
    #endregion // 프로퍼티 

    #region 함수 
    /** 초기화 */
    public virtual void Awake()
    {
        var RootObjs = this.gameObject.scene.GetRootGameObjects();

        for (int i = 0; i < RootObjs.Length; i++)
        {
            this.PublicRoot = this.PublicRoot ??
                RootObjs[i].transform.Find("Canvas/PublicRoot")?.gameObject;
        }
    }
    #endregion // 함수
}
