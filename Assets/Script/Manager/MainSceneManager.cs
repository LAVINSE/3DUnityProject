using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneManager : CSceneManager
{
    #region 변수
    [Header("=====> 카메라 <=====")]
    [SerializeField] private GameObject MenuCamera;
    [SerializeField] private GameObject PlayerCamera;

    [Header("=====> 플레이어 <=====")]
    [SerializeField] private PlayerAction Player;
    [SerializeField] private EnemyBoss Boss;

    [Header("=====> UI <=====")]
    [SerializeField] private GameObject ItemShopObject;
    [SerializeField] private GameObject WeaponShopObject;

    [Header("=====> 스폰 위치 <=====")]
    [SerializeField] private GameObject StartZoneObject;
    [SerializeField] private Transform SpawnPoint;

    [Header("=====> 설정 <=====")]
    [SerializeField] private int StageCount;
    [SerializeField] private float PlayTime;
    [SerializeField] private bool IsBattle;
    [SerializeField] private int EnemyCount;
    [SerializeField] private int EnemyCountD;

    [SerializeField] private Transform[] EnemyZoneArray;
    [SerializeField] private GameObject[] EnemyPrefabArray;
    [SerializeField] private List<int> EnemyList = new List<int>();

    [SerializeField] private GameObject MenuPanelObject;
    [SerializeField] private GameObject GamePanelObject;
    [SerializeField] private GameObject GameOverPanelObject;
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

    [SerializeField] private TMP_Text EnemyText;

    [SerializeField] private RectTransform BossHealthGroup;
    [SerializeField] private RectTransform BossHealthBar;

    [SerializeField] private TMP_Text CurrentScoreText;
    [SerializeField] private TMP_Text BestScoreText;

    [SerializeField] private List<GameObject> WallDoorList = new List<GameObject>(); 
    [SerializeField] private float WaitTimer; // 게임 시작전 대기 시간
    [SerializeField] private float FarmingTimer; // 게임 시작 파밍 시간
    [SerializeField] private float BattleTimer; // 게임 시작 파밍 이후 전투 시간 >> 빨리깨면 보상 up
    [SerializeField] private bool IsWaitTime;
    [SerializeField] private bool IsFarmingTime;
    [SerializeField] private bool IsBattleTime;

    private float SaveWaitTimer;
    private float SaveFarmingTimer;
    #endregion // 변수

    #region 프로퍼티
    public int oEnemyCount
    {
        get => EnemyCount;
        set => EnemyCount = value;
    }
    #endregion // 프로퍼티

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        SaveWaitTimer = WaitTimer;
        SaveFarmingTimer = FarmingTimer;
        MaxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
    }

    /** 초기화 => 상태를 갱신한다 */
    private void Update()
    {
        // 시간 설정
        if (IsWaitTime)
        {
            WaitTimer -= Time.deltaTime;
        }
        else if (IsFarmingTime)
        {
            FarmingTimer -= Time.deltaTime;
        }
        else if (IsBattleTime)
        {
            BattleTimer += Time.deltaTime;
        }

        WallDoorControl();

        // 전투가 시작될 경우
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
        StageText.text = "STAGE" + StageCount;
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
        EnemyText.text = EnemyCount.ToString();

        if(Boss != null)
        {
            BossHealthGroup.anchoredPosition = Vector3.down * 30;
            // 보스 체력바
            BossHealthBar.localScale = new Vector3((float)Boss.oCurrentHealth / Boss.oMaxHealth, 1, 1);
        }
        else
        {
            //BossHealthGroup.anchoredPosition = Vector3.down * 200;
        }
    }

    /** 버튼 >> 게임을 시작한다 */
    public void GameStart()
    {
        MenuCamera.SetActive(false);
        PlayerCamera.SetActive(true);

        MenuPanelObject.SetActive(false);
        GamePanelObject.SetActive(true);

        Player.gameObject.SetActive(true);
    }
    
    /** 게임을 종료한다 */
    public void GameOver()
    {
        GamePanelObject.SetActive(false);
        GameOverPanelObject.SetActive(true);
        CurrentScoreText.text = ScoreText.text;

        int MaxScore = PlayerPrefs.GetInt("MaxScore");
        if(Player.oScroe > MaxScore)
        {
            BestScoreText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", Player.oScroe);
        }
    }

    /** 게임 재시작 */
    public void ReStart()
    {
        SceneManager.LoadScene(0);
    }

    /** 스테이지 시작 */
    public void StageStart()
    {
        ItemShopObject.SetActive(false);
        WeaponShopObject.SetActive(false);
        StartZoneObject.SetActive(false);

        foreach(Transform ZonePos in EnemyZoneArray)
        {
            ZonePos.gameObject.SetActive(true);
        }

        IsBattle = true;
        StartCoroutine(StartBattelCo());
    }

    /** 스테이지 종료 */
    public void StageEnd()
    {
        Player.transform.position = SpawnPoint.position;

        ItemShopObject.SetActive(true);
        WeaponShopObject.SetActive(true);
        StartZoneObject.SetActive(true);

        foreach (Transform ZonePos in EnemyZoneArray)
        {
            ZonePos.gameObject.SetActive(false);
        }

        IsBattle = false;
        StageCount++;
    }

    // 전투 시작
    private IEnumerator StartBattelCo()
    {
        if (StageCount % 5 == 0)
        {
            GameObject EnemyObject = Instantiate(EnemyPrefabArray[3],
                EnemyZoneArray[0].position, EnemyZoneArray[0].rotation);
            Enemy EnemyComponent = EnemyObject.GetComponent<Enemy>();
            EnemyComponent.oPlayerTarget = Player.transform;
            EnemyComponent.oMainSceneManager = this;
            Boss = EnemyObject.GetComponent<EnemyBoss>();
            EnemyCount++;
        }
        else
        {
            for (int i = 0; i < StageCount; i++)
            {
                // 몬스터 종류 랜덤
                int Ran = Random.Range(0, 3);
                EnemyList.Add(Ran);

                EnemyCount++;
            }
        }

        // 4초마다 소환
        while (EnemyList.Count > 0)
        {
            // 스폰위치 랜덤
            int RandomZone = Random.Range(0, 4);
            GameObject EnemyObject = Instantiate(EnemyPrefabArray[EnemyList[0]],
                EnemyZoneArray[RandomZone].position, EnemyZoneArray[RandomZone].rotation);
            Enemy EnemyComponent = EnemyObject.GetComponent<Enemy>();
            EnemyComponent.oPlayerTarget = Player.transform;
            EnemyComponent.oMainSceneManager = this;
            EnemyList.RemoveAt(0);
            yield return new WaitForSeconds(4f);
        }
        

        while (EnemyCount > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4f);

        Boss = null;
        StageEnd();
    }

    /** 움직히는 문을 컨트롤 한다 */
    private void WallDoorControl()
    {
        // 0 초가 됐을 경우
        if(WaitTimer <= 0 && IsWaitTime)
        {
            IsWaitTime = false;
            WaitTimer = SaveWaitTimer;
            for (int i = 0; i < WallDoorList.Count; i++)
            {
                WallDoorList[i].transform.DOMove(WallDoorList[i].transform.position + Vector3.down * 15f,
                    2f);
            }
            IsFarmingTime = true;
        }

        IsFarming();
    }

    /** 파밍이 끝났는지 확인 */
    private void IsFarming()
    {
        if(FarmingTimer <= 0 && IsFarmingTime)
        {
            IsFarmingTime = false;
            FarmingTimer = SaveFarmingTimer;
            IsBattleTime = true;
        }

        StartBattle();
    }

    /** 전투 시작 */
    private void StartBattle()
    {
        if(IsBattleTime && BattleTimer == 0)
        {
            Debug.Log("성공");
            // 전투 시작
            // 코루틴 시작
        }
    }
    #endregion // 함수
}
