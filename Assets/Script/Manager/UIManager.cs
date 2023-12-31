using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region 변수
    [Header("=====> 메뉴 UI <=====")]
    public GameObject MenuPanelUI;
    public GameObject GameOverPanelUI;
    public GameObject GameClearPanelUI;
    [SerializeField] private GameObject MenuScoreTextRootObject;
    [SerializeField] private GameObject MenuScoreTextPrefab;
    [SerializeField] private GameObject ClearScoreTextRootObject;
    [SerializeField] private GameObject ClearScoreTextPrefab;
    [Header("=====> 인게임 UI <=====")]
    public GameObject InGamePanelUI;

    [Header("=====> 상태창 UI <=====")]
    [SerializeField] private TMP_Text PlayerHealthText;
    [SerializeField] private TMP_Text PlayerAmmoText;
    [SerializeField] private TMP_Text PlayerCoinText;

    [Header("=====> 장비창 UI <=====")]
    [SerializeField] private Image Weapon_1Img;
    [SerializeField] private Image Weapon_2Img;
    [SerializeField] private Image Weapon_3Img;
    [SerializeField] private Image Weapon_MouseRImg;

    [Header("=====> 스테이지 UI <=====")]
    [SerializeField] private GameObject BossStatusGroup;
    [SerializeField] private RectTransform BossHealthBarImg;
    [SerializeField] private TMP_Text StageName;
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private TMP_Text EnemyCountText;
    [SerializeField] private TMP_Text EnemyBossCountText;

    private float WaitTimer;
    private float FarmingTimer;
    private float BattleTimer; 
    #endregion // 변수

    #region 프로퍼티
    public static UIManager Instance { get; private set; }
    public MainSceneManager oMainSceneManager { get; private set; }
    public PlayerAction oPlayer { get; private set; }
    public StoneStatue oStoneStatue { get; private set; }
    #endregion // 프로퍼티

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        Instance = this;
        oMainSceneManager = CSceneManager.GetSceneManager<MainSceneManager>(CDefine.MainGameScene);
        oPlayer = oMainSceneManager.PlayerObj.GetComponent<PlayerAction>();

        // 상태창 갱신
        PlayerStatusTextUpdate();
        PlayerAmmoTextUpdate();
        PlayerWeaponImgUpdate();
    }

    /** 초기화 */
    private void Start()
    {
        MenuScoreTextSetting();
    }

    /** 초기화 => 상태를 갱신한다 */
    private void LateUpdate()
    {
        WaitTimer = oMainSceneManager.oWaitTimer;
        FarmingTimer = oMainSceneManager.oFarmingTimer;
        BattleTimer = oMainSceneManager.oBattleTimer;

        if (oMainSceneManager.oIsWaitTime)
        {
            TimerTextSetting(WaitTimer);
        }
        else if (oMainSceneManager.oIsFarmingTime)
        {
            TimerTextSetting(FarmingTimer);
        }
        else if (oMainSceneManager.oIsBattleTime)
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

    /** 메인메뉴 시간을 나타내는 텍스트를 설정한다 */
    public void MenuScoreTextSetting()
    {
        List<GameObject> TextObjectList = new List<GameObject>();

        for (int i = 0; i < oMainSceneManager.StageData.StageArray.Length; i++)
        {
            var Text = CFactory.CreateCloneObj("ScoreText", MenuScoreTextPrefab, MenuScoreTextRootObject,
                Vector3.zero, Vector3.one, Vector3.zero);
            TextObjectList.Add(Text);
        }

        for (int i = 0; i < oMainSceneManager.MenuClearBattleTimer.Count; i++)
        {
            var Timer = oMainSceneManager.MenuClearBattleTimer[i];

            int Hour = (int)(Timer / 3600);
            int Min = (int)((Timer - Hour * 3600) / 60);
            int Second = (int)(Timer % 60);

            TextObjectList[i].GetComponent<TMP_Text>().text = $"{oMainSceneManager.StageData.StageArray[i].StageName}" + " " +
                " | " + string.Format("{0:00}", Hour) + ":" + string.Format("{0:00}", Min) + ":" + string.Format("{0:00}", Second);
        }
    }

    /** 클리어 시간을 나타내는 텍스트를 설정한다 */
    public void ClearScoreText()
    {
        for(int i = 0; i < oMainSceneManager.StageClearBattleTimer.Count; i++)
        {
            var Text = CFactory.CreateCloneObj("ScoreText", MenuScoreTextPrefab, MenuScoreTextRootObject,
                Vector3.zero, Vector3.one, Vector3.zero);

            var Timer = oMainSceneManager.StageClearBattleTimer[i];

            int Hour = (int)(Timer / 3600);
            int Min = (int)((Timer - Hour * 3600) / 60);
            int Second = (int)(Timer % 60);

            Text.GetComponent<TMP_Text>().text = $"{oMainSceneManager.StageData.StageArray[i].StageName}" + " " +
                " | " + string.Format("{0:00}", Hour) + ":" + string.Format("{0:00}", Min) + ":" + string.Format("{0:00}", Second);
        }
    }

    /** 상태창을 갱신한다 */
    public void PlayerStatusTextUpdate()
    {
        PlayerHealthText.text = oPlayer.oHealth + " / " + oPlayer.oMaxHealth;
        PlayerAmmoText.text = oPlayer.oAmmo + " / " + oPlayer.oMaxAmmo;
        PlayerCoinText.text = string.Format("{0:n0}", oPlayer.oCoin);
    }

    /** 탄약정보를 갱신한다 */
    public void PlayerAmmoTextUpdate()
    {
        if (oPlayer.oEquipWeapon == null)
        {
            PlayerAmmoText.text = "- / " + oPlayer.oAmmo;
        }
        else if (oPlayer.oEquipWeapon.oType == Weapon.WeaponType.Melee)
        {
            PlayerAmmoText.text = "- / " + oPlayer.oAmmo;
        }
        else // 탄약 사용하는 원거리 무기
        {
            PlayerAmmoText.text = oPlayer.oEquipWeapon.oCurrentAmmo + " / " + oPlayer.oAmmo;
        }
    }

    /** 무기 이미지를 갱신한다 */
    public void PlayerWeaponImgUpdate()
    {
        // 무기 이미지 설정
        Weapon_1Img.color = new Color(1, 1, 1, oPlayer.oHasWeaponArray[0] ? 1 : 0);
        Weapon_2Img.color = new Color(1, 1, 1, oPlayer.oHasWeaponArray[1] ? 1 : 0);
        Weapon_3Img.color = new Color(1, 1, 1, oPlayer.oHasWeaponArray[2] ? 1 : 0);
        Weapon_MouseRImg.color = new Color(1, 1, 1, oPlayer.oHasGrenades > 0 ? 1 : 0);
    }

    /** 몬스터 숫자를 갱신한다 */
    public void EnemyCountTextUpdate()
    {
        EnemyCountText.text = oMainSceneManager.oEnemyCount.ToString();
        EnemyBossCountText.text = oMainSceneManager.oEnemyBossCount.ToString();
    }

    /** 보스 상태창을 갱신한다 */
    public void BossHealthBarUpdate()
    {
        if (oMainSceneManager.oBossObject != null)
        {
            var BossComponent = oMainSceneManager.oBossObject.GetComponent<Enemy>();
            BossStatusGroup.SetActive(true);
            // 보스 체력바
            BossHealthBarImg.localScale = new Vector3((float)BossComponent.oCurrentHealth / BossComponent.oMaxHealth, 1, 1);
        }
        else
        {
            BossStatusGroup.SetActive(false);
        }
    }
    #endregion // 함수
}
