using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region 변수
    [Header("=====> 메뉴 UI <=====")]
    [SerializeField] private GameObject MenuPanelUI;

    [Header("=====> 인게임 UI <=====")]
    [SerializeField] private GameObject InGamePanelUI;

    [Header("=====> 상태창 UI <=====")]
    [SerializeField] private TMP_Text HealthText;
    [SerializeField] private TMP_Text AmmoText;
    [SerializeField] private TMP_Text CoinText;

    [Header("=====> 장비창 UI <=====")]
    [SerializeField] private Image Weapon_1Img;
    [SerializeField] private Image Weapon_2Img;
    [SerializeField] private Image Weapon_3Img;
    [SerializeField] private Image Weapon_MouseRImg;

    [Header("=====> 스테이지 UI <=====")]
    [SerializeField] private GameObject BossStatusGroup;
    [SerializeField] private TMP_Text StageName;
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private TMP_Text EnemyCountText;

    public float WaitTimer;
    public float FarmingTimer;
    public float BattleTimer;
    #endregion // 변수

    #region 프로퍼티
    public static UIManager Instance { get; private set; }
    #endregion // 프로퍼티

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        Instance = this;
    }

    /** 초기화 => 상태를 갱신한다 */
    private void LateUpdate()
    {
        var SceneManagerComponent = CSceneManager.GetSceneManager<MainSceneManager>(CDefine.MainGameScene);
        WaitTimer = SceneManagerComponent.WaitTimer;
        FarmingTimer = SceneManagerComponent.FarmingTimer;
        BattleTimer = SceneManagerComponent.BattleTimer;

        if (SceneManagerComponent.IsWaitTime)
        {
            TimerTextSetting(WaitTimer);
        }
        else if (SceneManagerComponent.IsFarmingTime)
        {
            TimerTextSetting(FarmingTimer);
        }
        else if (SceneManagerComponent.IsBattleTime)
        {
            TimerTextSetting(BattleTimer);
        }
    }

    /** 시간을 나타내는 텍스트를 설정한다 */
    private void TimerTextSetting(float Timer)
    {
        int Hour = (int)(Timer / 3600);
        int Min = (int)((Timer - Hour * 3600) / 60);
        int Second = (int)(Timer % 60);

        TimerText.text = string.Format("{0:00}", Hour) + ":" + string.Format("{0:00}", Min)
            + ":" + string.Format("{0:00}", Second);
    }
    #endregion // 함수
}
