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

    public float WaitTimer;
    public float FarmingTimer;
    public float BattleTimer;
    public MainSceneManager oMainSceneManager { get; private set; }
    public PlayerAction oPlayer { get; private set; }
    #endregion // 변수

    #region 프로퍼티
    public static UIManager Instance { get; private set; }
    #endregion // 프로퍼티

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        Instance = this;
        oMainSceneManager = CSceneManager.GetSceneManager<MainSceneManager>(CDefine.MainGameScene);
        oPlayer = oMainSceneManager.PlayerObj.GetComponent<PlayerAction>();
        HealthTextUpdate();
        AmmoTextUpdate();
    }

    /** 초기화 => 상태를 갱신한다 */
    private void LateUpdate()
    {
        WaitTimer = oMainSceneManager.WaitTimer;
        FarmingTimer = oMainSceneManager.FarmingTimer;
        BattleTimer = oMainSceneManager.BattleTimer;

        if (oMainSceneManager.IsWaitTime)
        {
            TimerTextSetting(WaitTimer);
        }
        else if (oMainSceneManager.IsFarmingTime)
        {
            TimerTextSetting(FarmingTimer);
        }
        else if (oMainSceneManager.IsBattleTime)
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

    /** 상태창을 갱신한다 */
    public void HealthTextUpdate()
    {
        PlayerHealthText.text = oPlayer.oHealth + " / " + oPlayer.oMaxHealth;
        PlayerAmmoText.text = oPlayer.oAmmo + " / " + oPlayer.oMaxAmmo;
        PlayerCoinText.text = string.Format("{0:n0}", oPlayer.oCoin);
    }

    /** 탄약정보를 갱신한다 */
    public void AmmoTextUpdate()
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
    public void WeaponImgUpdate()
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
    }

    /** 보스 상태창을 갱신한다 */
    public void BossHealthBarUpdate()
    {
        if (oMainSceneManager.BossObj != null)
        {
            var BossComponent = oMainSceneManager.BossObj.GetComponent<Enemy>();
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
