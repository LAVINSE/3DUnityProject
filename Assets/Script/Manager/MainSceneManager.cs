using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneManager : SceneManager
{
    #region 변수
    [SerializeField] private GameObject MenuCamera;
    [SerializeField] private GameObject PlayerCamera;
    [SerializeField] private PlayerAction Player;
    [SerializeField] private EnemyBoss Boss;
    [SerializeField] private int Statge;
    [SerializeField] private float PlayTime;
    [SerializeField] private bool IsBattle;
    [SerializeField] private int EnemyCountA;
    [SerializeField] private int EnemyCountB;
    [SerializeField] private int EnemyCountC;

    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private GameObject GamePanel;
    [SerializeField] private TMP_Text MaxScoreText;

    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private TMP_Text StageText;
    [SerializeField] private TMP_Text PlayTimeText;
    [SerializeField] private TMP_Text PlayerHealthText;
    [SerializeField] private TMP_Text PlayerAmmoText;
    [SerializeField] private TMP_Text PlayerCoinText;

    [SerializeField] private Image Weapon_1_Img;
    [SerializeField] private Image Weapon_2_Img;
    [SerializeField] private Image Weapon_3_Img;
    [SerializeField] private Image Weapon_R_Img;

    [SerializeField] private TMP_Text EnemyAText;
    [SerializeField] private TMP_Text EnemyBText;
    [SerializeField] private TMP_Text EnemyCText;

    [SerializeField] private RectTransform BossHealthGroup;
    [SerializeField] private RectTransform BossHealthBar;
    #endregion // 변수

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        MaxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
    }

    /** 초기화 => 상태를 갱신한다 */
    private void Update()
    {
        if (IsBattle)
        {
            PlayTime += Time.deltaTime;
        }
    }

    /** 초기화 => 상태를 갱신한다 */
    private void LateUpdate()
    {
        int Hour = (int)(PlayTime / 3600);
        int Min = (int)((PlayTime - Hour * 3600) / 60);
        int Second = (int)(PlayTime % 60);

        // 텍스트 설정
        PlayTimeText.text = string.Format("{0:00}", Hour) + ":" + string.Format("{0:00}", Min) 
            + ":" + string.Format("{0:00}", Second);
        StageText.text = "STAGE" + Statge;
        ScoreText.text = string.Format("{0:n0}", Player.oScroe);

        // 플레이어 UI
        PlayerHealthText.text = Player.oHealth + " / " + Player.oMaxHealth;
        PlayerCoinText.text = string.Format("{0:n0}", Player.oCoin);
        PlayerAmmoText.text = Player.oAmmo + " / " + Player.oMaxAmmo;
        if(Player.oEquipWeapon == null)
        {
            PlayerAmmoText.text = "- / " + Player.oAmmo;
        }
        else if(Player.oEquipWeapon.oType == Weapon.WeaponType.Melee)
        {
            PlayerAmmoText.text = "- / " + Player.oAmmo;
        }
        else
        {
            PlayerAmmoText.text = Player.oEquipWeapon.oCurrentAmmo + " / " + Player.oAmmo;
        }

        // 무기 이미지 설정
        Weapon_1_Img.color = new Color(1, 1, 1, Player.oHasWeaponArray[0] ? 1 : 0);
        Weapon_2_Img.color = new Color(1, 1, 1, Player.oHasWeaponArray[1] ? 1 : 0);
        Weapon_3_Img.color = new Color(1, 1, 1, Player.oHasWeaponArray[2] ? 1 : 0);
        Weapon_R_Img.color = new Color(1, 1, 1, Player.oHasGrenades > 0 ? 1 : 0);

        // 몬스터 숫자 UI
        EnemyAText.text = EnemyCountA.ToString();
        EnemyBText.text = EnemyCountB.ToString();
        EnemyCText.text = EnemyCountC.ToString();

        // 보스 체력바
        BossHealthBar.localScale = new Vector3((float)Boss.oCurrentHealth / Boss.oMaxHealth, 1, 1);
    }

    /** 게임을 시작한다 */
    public void GameStart()
    {
        MenuCamera.SetActive(false);
        PlayerCamera.SetActive(true);

        MenuPanel.SetActive(false);
        GamePanel.SetActive(true);

        Player.gameObject.SetActive(true);
    }
    #endregion // 함수
}
