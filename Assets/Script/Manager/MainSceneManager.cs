using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;

public class MainSceneManager : CSceneManager
{
    #region 변수
    [Header("=====> 카메라 <=====")]
    [SerializeField] private GameObject MenuCamera;
    [SerializeField] private GameObject PlayerCamera;

    [Header("=====> 플레이어 <=====")]
    [SerializeField] private PlayerAction Player;
    [SerializeField] private GameObject PlayerInven;
    
    [Header("=====> 상호작용 오브젝트 <=====")]
    [SerializeField] private GameObject ItemShopObject;
    [SerializeField] private GameObject WeaponShopObject;
    [SerializeField] private GameObject UpgradShopObject;

    [Header("=====> 스폰 위치 <=====")]
    [SerializeField] private Transform PlayerSpawnPos;

    [Header("=====> 게임 설정 <=====")]
    public StageDataTable StageData;
    [SerializeField] private List<GameObject> WallDoorList = new List<GameObject>();
    [SerializeField] private List<GameObject> EnemySpawnZoneList = new List<GameObject>();
    [SerializeField] private List<GameObject> EnemyBossSpawnZoneList = new List<GameObject>();
    
    [Space]
    [SerializeField] private float WaitTimer; // 게임 시작전 대기 시간
    [SerializeField] private float FarmingTimer; // 게임 시작 파밍 시간
    [SerializeField] private float BattleTimer; // 게임 시작 파밍 이후 전투 시간 >> 빨리깨면 보상 up
    [SerializeField] private bool IsWaitTime;
    [SerializeField] private bool IsFarmingTime;
    [SerializeField] private bool IsBattleTime;

    [Space]
    [SerializeField] private int EnemyCount;
    [SerializeField] private int EnemyBossCount;
    [SerializeField] private int StageCount;
    [SerializeField] private int MaxStageCount;
    [SerializeField] private GameObject BossObject;

    [Header("=====> 설정 <=====")]
    public List<float> StageClearBattleTimer = new List<float>();
    [SerializeField] private bool IsEnemyDie;
    [SerializeField] private float SpawnTime;

    private float SaveWaitTimer;
    private float SaveFarmingTimer;
    private List<int> NumList = new List<int>();
    #endregion // 변수

    #region 프로퍼티
    public override string SceneName => CDefine.MainGameScene;
    public float oWaitTimer => WaitTimer;
    public float oFarmingTimer => FarmingTimer;
    public float oBattleTimer => BattleTimer;
    public bool oIsWaitTime => IsWaitTime;
    public bool oIsFarmingTime => IsFarmingTime;
    public bool oIsBattleTime => IsBattleTime;
    public int oEnemyCount
    {
        get => EnemyCount;
        set => EnemyCount = value;
    }
    public int oEnemyBossCount
    {
        get => EnemyBossCount;
        set => EnemyBossCount = value;
    }
    public GameObject oBossObject => BossObject;
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

        Player.transform.position = PlayerSpawnPos.position;

        SaveWaitTimer = WaitTimer;
        SaveFarmingTimer = FarmingTimer;

        // 배틀타이머 가져오기
        for (int i = 0; i < StageData.StageArray.Length; i++)
        {
            StageClearBattleTimer.Add(PlayerPrefs.GetFloat($"ScoreText{i}"));
        }
    }

    /** 초기화 => 상태를 갱신한다 */
    private void Update()
    {
        if (IsEnemyDie)
        {
            EnemyCount = 0;
            EnemyBossCount = 0;
            IsEnemyDie = false;
        }

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

        IsWating();
    }

    /** 버튼 >> 게임을 시작한다 */
    public void GameStart()
    {
        // 카메라 활성 제어
        MenuCamera.SetActive(false);
        PlayerCamera.SetActive(true);

        UIManager.Instance.MenuPanelUI.SetActive(false);
        UIManager.Instance.InGamePanelUI.SetActive(true);

        Player.gameObject.SetActive(true);

        GameManager.Inst.CursorLock();
        IsWaitTime = true;
    }
    
    /** 게임을 종료한다 */
    public void GameOver()
    {
        UIManager.Instance.InGamePanelUI.SetActive(false);
        UIManager.Instance.GameOverPanelUI.SetActive(true);
    }

    /** 게임을 클리어한다 */
    public void GameClear()
    {
        UIManager.Instance.ClearScoreText();
        UIManager.Instance.InGamePanelUI.SetActive(false);
        UIManager.Instance.GameClearPanelUI.SetActive(true);
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

        ItemShopObject.SetActive(false);
        WeaponShopObject.SetActive(false);
        UpgradShopObject.SetActive(false);

        StartCoroutine(StartBattelCo());
    }

    /** 스테이지 종료 */
    public void StageEnd()
    {
        Debug.Log("스테이지 종료");
        // 랜덤리스트 초기화
        NumList.Clear();

        StageClearBattleTimer.Add(BattleTimer);
        
        // 배틀타이머 저장, 초기화
        for (int i = 0; i < StageClearBattleTimer.Count; i++)
        {
            PlayerPrefs.SetFloat($"ScoreText{i}", StageClearBattleTimer[i]);
        }
         
        BattleTimer = 0;

        IsBattleTime = false;

        Player.transform.position = PlayerSpawnPos.transform.position;

        // 문 올리기
        NextStageWallDoorUp();

        ItemShopObject.SetActive(true);
        WeaponShopObject.SetActive(true);
        UpgradShopObject.SetActive(true);

        IsWaitTime = true;

        // 스테이지 카운트 증가
        StageCount++;

        if(StageCount == MaxStageCount)
        {
            GameClear();
        }
    }

    /** 스테이지가 바뀌고 문이 올라간다 */
    private void NextStageWallDoorUp()
    {
        for (int i = 0; i < WallDoorList.Count; i++)
        {
            WallDoorList[i].transform.DOMove(WallDoorList[i].transform.position + Vector3.up * 15f,
                1f);
        }
    }

    /** 대기시간이 끝났을경우 문을 닫는다 */
    private void IsWating()
    {
        // 0 초가 됐을 경우
        if (WaitTimer <= 0 && IsWaitTime)
        {
            IsWaitTime = false;
            WaitTimer = SaveWaitTimer;
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

            for (int i = 0; i < WallDoorList.Count; i++)
            {
                WallDoorList[i].transform.DOMove(WallDoorList[i].transform.position + Vector3.down * 15f,
                    2f);
            }

            IsBattleTime = true;
            StartBattle();
        }  
    }

    /** 전투 시작 */
    private void StartBattle()
    {
        if (IsBattleTime && BattleTimer == 0)
        {
            Debug.Log("전투 진입");
            StageStart();
        }
    }

    // 전투 시작
    private IEnumerator StartBattelCo()
    {
        List<GameObject> ZoneList = new List<GameObject>();

        // 적 스폰존 활성화
        for(int i =0; i < StageData.StageArray[StageCount].StageSpawnActiveCount; i++)
        {
            int Number = RandomNumber(0, EnemySpawnZoneList.Count);
            var EnemyZone = EnemySpawnZoneList[Number];
            ZoneList.Add(EnemyZone);
        }

        NumList.Clear();

        // 적 소환
        int Count = 0;
        while (Count < StageData.StageArray[StageCount].StageEnemyCount)
        {
            int Number = RandomNumber(0, ZoneList.Count);
            int EnemyRand = Random.Range(0, StageData.StageArray[StageCount].EnemyPrefabList.Length);
            ZoneList[Number].SetActive(true);

            GameObject EnemyObject = CFactory.CreateCloneObj("Enemy",
                                    StageData.StageArray[StageCount].EnemyPrefabList[EnemyRand],
                                    ZoneList[Number], Vector3.zero, Vector3.one, Vector3.zero);

            EnemyObject.GetComponent<Enemy>().oSpawnPos = ZoneList[Number].transform;
            EnemyObject.GetComponent<Enemy>().oPlayerTarget = Player.transform;
            bool Ishit = NavMesh.SamplePosition(EnemyObject.transform.position, out NavMeshHit Hit, float.MaxValue / 2, 1);
            
            if (Ishit)
            {
                EnemyObject.transform.position = Hit.position;
                EnemyObject.GetComponent<Enemy>().oEnemyNavMeshAgent.enabled = true;
            }

            EnemyCount++;
            Count++;

            // 적 숫자 상태창 업데이트
            UIManager.Instance.EnemyCountTextUpdate();

            // 4초 마다 소환
            yield return new WaitForSeconds(SpawnTime);
        }

        while (EnemyCount > 0)
        {
            Debug.Log($"적이 {EnemyCount}만큼 남았습니다");
            yield return null;
        }

        // 일반 몹을 다 잡았을 경우, 보스소환
        if(EnemyCount == 0)
        {
            int ZoneRand = Random.Range(0, EnemyBossSpawnZoneList.Count);
            var BossZone = EnemyBossSpawnZoneList[ZoneRand];
            BossZone.SetActive(true);
           
            GameObject EnemyObject = Instantiate(StageData.StageArray[StageCount].EnemyBoss, BossZone.transform);
            BossObject = EnemyObject;
            EnemyObject.GetComponent<Enemy>().oSpawnPos = BossZone.transform;
            EnemyObject.GetComponent<Enemy>().oPlayerTarget = Player.transform;
            bool Ishit = NavMesh.SamplePosition(EnemyObject.transform.position, out NavMeshHit Hit, float.MaxValue / 2, 1);

            if (Ishit)
            {
                EnemyObject.transform.position = Hit.position;
                EnemyObject.GetComponent<Enemy>().oEnemyNavMeshAgent.enabled = true;
            }
            
            EnemyBossCount++;

            // 보스 체력 상태창 업데이트
            UIManager.Instance.BossHealthBarUpdate();
            // 적 숫자 상태창 업데이트
            UIManager.Instance.EnemyCountTextUpdate();
        }


        while (EnemyBossCount > 0)
        {
            Debug.Log($"보스가 {EnemyBossCount} 만큼 남았습니다");
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        StageEnd();
    }

    /** 랜덤숫자중 중복 숫자를 뽑지 않는다 */
    private int RandomNumber(int Min, int Max)
    {
        int RandomValue;
        do
        {
            RandomValue = Random.Range(Min, Max);
        } while (NumList.Contains(RandomValue));

        NumList.Add(RandomValue);
        return RandomValue;
    }
    #endregion // 함수
}
