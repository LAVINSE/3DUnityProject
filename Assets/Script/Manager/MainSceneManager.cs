using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject PlayerInven;
    [SerializeField] private GameObject Boss;

    [Header("=====> UI <=====")]
    [SerializeField] private GameObject ItemShopObject;
    [SerializeField] private GameObject WeaponShopObject;

    [Header("=====> 스폰 위치 <=====")]
    [SerializeField] private GameObject StartZoneObject;
    [SerializeField] private Transform SpawnPoint;

    [Header("=====> 설정 <=====")]
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
    [SerializeField] public float WaitTimer; // 게임 시작전 대기 시간
    [SerializeField] public float FarmingTimer; // 게임 시작 파밍 시간
    [SerializeField] public float BattleTimer; // 게임 시작 파밍 이후 전투 시간 >> 빨리깨면 보상 up
    [SerializeField] public bool IsWaitTime;
    [SerializeField] public bool IsFarmingTime;
    [SerializeField] public bool IsBattleTime;
    [SerializeField] public GameObject StatusObj;
    [SerializeField] private List<GameObject> EnemySpawnZoneList = new List<GameObject>();
    [SerializeField] private StageDataTable StageData;
    [SerializeField] private int StageCount;
    private float SaveWaitTimer;
    private float SaveFarmingTimer;
    #endregion // 변수

    #region 프로퍼티
    public override string SceneName => CDefine.MainGameScene;
    public int oEnemyCount
    {
        get => EnemyCount;
        set => EnemyCount = value;
    }
    #endregion // 프로퍼티

    #region 함수
    /** 초기화 */
    public override void Awake()
    {
        base.Awake();
        PlayerObj = Player.gameObject;
        var InventoryObject = CFactory.CreateCloneObj("PlayerInventory", PlayerInven, PublicRoot,
            Vector3.zero, Vector3.one, Vector3.zero);
        var InventoryComponent = InventoryObject.GetComponent<Inventory>();
        InventoryComponent.CloseInventory();
        Player.Inven = InventoryComponent;

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
    }

    /** 초기화 => 상태를 갱신한다 */
    private void LateUpdate()
    {
        StageText.text = "STAGE" + StageCount;
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
        Debug.Log("스테이지 시작");
        StartCoroutine(StartBattelCo());
    }

    /** 스테이지 종료 */
    public void StageEnd()
    {
        Debug.Log("스테이지 종료");

        IsBattleTime = false;
        StageCount++;
    }

    /** 움직히는 문을 컨트롤 한다 */
    private void WallDoorControl()
    {
        // 0 초가 됐을 경우
        if (WaitTimer <= 0 && IsWaitTime)
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
        if (FarmingTimer <= 0 && IsFarmingTime)
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
        if (IsBattleTime && BattleTimer == 0)
        {
            Debug.Log("성공");
            StageStart();
        }
    }

    // 전투 시작
    private IEnumerator StartBattelCo()
    {
        List<GameObject> ZoneList = new List<GameObject>();

        if(StageCount % 5 == 0)
        {

        }
        // 적 스폰존 활성화
        for(int i =0; i < StageData.StageArray[StageCount].StageSpawnActiveCount; i++)
        {
            int Rand = Random.Range(0, EnemySpawnZoneList.Count);
            var EnemyZone = EnemySpawnZoneList[Rand];
            ZoneList.Add(EnemyZone);
        }

        // 적 소환
        int Count = 0;
        while (Count <= StageData.StageArray[StageCount].StageEnemyCount)
        {
            int Rand = Random.Range(0, ZoneList.Count);
            int EnemyRand = Random.Range(0, StageData.StageArray[StageCount].EnemyPrefabList.Length);
            ZoneList[Rand].SetActive(true);
            GameObject EnemyObject = CFactory.CreateCloneObj("Enemy",
                                    StageData.StageArray[StageCount].EnemyPrefabList[EnemyRand],
                                    ZoneList[Rand], Vector3.zero, Vector3.one, Vector3.zero);
            EnemyObject.GetComponent<Enemy>().oPlayerTarget = Player.transform;
            EnemyObject.GetComponent<Enemy>().oStoneStatueTarget = StatusObj.transform;
            EnemyCount++;
            yield return new WaitForSeconds(4f);
        }

        while (EnemyCount > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4f);

        StageEnd();
    }
    #endregion // 함수
}
