using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CSceneManager : MonoBehaviour
{
    #region 프로퍼티
    private static Dictionary<string, CSceneManager> SceneManagerDict = new Dictionary<string, CSceneManager>();
    public abstract string SceneName { get; }

    public GameObject PublicRoot { get; private set; }
    public GameObject PlayerObj { get; set; }
    public GameObject BossObj { get; set; }
    #endregion // 프로퍼티 

    #region 함수 
    /** 초기화 */
    public virtual void Awake()
    {
        CSceneManager.SceneManagerDict.TryAdd(this.SceneName, this);

        var RootObjs = this.gameObject.scene.GetRootGameObjects();   

        for (int i = 0; i < RootObjs.Length; i++)
        {
            this.PublicRoot = this.PublicRoot ??
                RootObjs[i].transform.Find("Canvas/PublicRoot")?.gameObject;
        }
    }

    /** 초기화 => 제거 되었을 경우 */
    public virtual void OnDestroy()
    {
        if(CSceneManager.SceneManagerDict.ContainsKey(this.SceneName))
        {
            CSceneManager.SceneManagerDict.Remove(this.SceneName);
        }
    }

    /** 씬 관리자를 반환한다 */
    public static T GetSceneManager<T>(string SceneName) where T : CSceneManager
    {
        return CSceneManager.SceneManagerDict.GetValueOrDefault(SceneName) as T;
    }
    #endregion // 함수
}
