using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
    
    [Header("=====> 상호작용 오브젝트 <=====")]
    [SerializeField] private GameObject ItemShopObject;
    [SerializeField] private GameObject WeaponShopObject;

    [Header("=====> 스폰 위치 <=====")]
    [SerializeField] private Transform PlayerSpawnPos;

    [Header("=====> 게임 설정 <=====")]
    [SerializeField] private StageDataTable StageData;
    [SerializeField] private List<GameObject> WallDoorList = new List<GameObject>();
    [SerializeField] private List<GameObject> EnemySpawnZoneList = new List<GameObject>();
    [SerializeField] private List<GameObject> EnemyBossSpawnZoneList = new List<GameObject>();
    [SerializeField] private GameObject StoneStatueObject;
    [SerializeField] private GameObject StoneStatueTargetPos;
    
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
    [SerializeField] private GameObject BossObject;

    [Header("=====> 설정 <=====")]
    [SerializeField] private GameObject MenuPanelObject;
    [SerializeField] private GameObject GamePanelObject;
    [SerializeField] private GameObject GameOverPanelObject;
    [SerializeField] private List<float> StageClearBattleTimer = new List<float>();
    [SerializeField] private bool IsEnemyDie;

    private float SaveWaitTimer;
    private float SaveFarmingTimer;
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
    public GameObject oStoneStatueObject => StoneStatueObject;
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

        SaveWaitTimer = WaitTimer;
        SaveFarmingTimer = FarmingTimer;
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

        WaitTimeWallDoorDown();
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

        StageClearBattleTimer.Add(BattleTimer);
        BattleTimer = 0;

        IsBattleTime = false;

        // 문 올리기
        NextStageWallDoorUp();

        IsWaitTime = true;
        StageCount++;
    }

    /** 대기시간이 끝났을경우 문을 닫는다 */
    private void WaitTimeWallDoorDown()
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

    /** 스테이지가 바뀌고 문이 올라간다 */
    private void NextStageWallDoorUp()
    {
        for (int i = 0; i < WallDoorList.Count; i++)
        {
            WallDoorList[i].transform.DOMove(WallDoorList[i].transform.position + Vector3.up * 15f,
                1f);
        }
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

        // 적 스폰존 활성화
        for(int i =0; i < StageData.StageArray[StageCount].StageSpawnActiveCount; i++)
        {
            int Rand = Random.Range(0, EnemySpawnZoneList.Count);
            var EnemyZone = EnemySpawnZoneList[Rand];
            ZoneList.Add(EnemyZone);
        }

        // 적 소환
        int Count = 0;
        while (Count < StageData.StageArray[StageCount].StageEnemyCount)
        {
            int Rand = Random.Range(0, ZoneList.Count);
            int EnemyRand = Random.Range(0, StageData.StageArray[StageCount].EnemyPrefabList.Length);
            ZoneList[Rand].SetActive(true);
            GameObject EnemyObject = CFactory.CreateCloneObj("Enemy",
                                    StageData.StageArray[StageCount].EnemyPrefabList[EnemyRand],
                                    ZoneList[Rand], Vector3.zero, Vector3.one, Vector3.zero);
            EnemyObject.GetComponent<Enemy>().oEnemyNavMeshAgent.enabled = false;
            EnemyObject.GetComponent<Enemy>().oPlayerTarget = Player.transform;
            EnemyObject.GetComponent<Enemy>().oStoneStatueTarget = StoneStatueTargetPos.transform;
            EnemyCount++;
            Count++;

            // 적 숫자 상태창 업데이트
            UIManager.Instance.EnemyCountTextUpdate();

            // 4초 마다 소환
            yield return new WaitForSeconds(2f);
            EnemyObject.GetComponent<Enemy>().oEnemyNavMeshAgent.enabled = true;
            yield return new WaitForSeconds(2f);
        }

        while (EnemyCount > 0)
        {
            Debug.Log($"적이 {EnemyCount}만큼 남았습니다");
            yield return null;
        }

        // 일반 몹을 다 잡았을 경우
        if(EnemyCount == 0)
        {
            int ZoneRand = Random.Range(0, EnemyBossSpawnZoneList.Count);
            var BossZone = EnemyBossSpawnZoneList[ZoneRand];
            BossZone.SetActive(true);
            GameObject EnemyObject = CFactory.CreateCloneObj("Enemy",
                                    StageData.StageArray[StageCount].EnemyBoss, BossZone
                                    , Vector3.zero, Vector3.one, Vector3.zero);

            EnemyObject.GetComponent<Enemy>().oEnemyNavMeshAgent.enabled = false;
            EnemyObject.GetComponent<Enemy>().oPlayerTarget = Player.transform;
            EnemyObject.GetComponent<Enemy>().oStoneStatueTarget = StoneStatueTargetPos.transform;
            EnemyBossCount++;
        }


        while (EnemyBossCount > 0)
        {
            Debug.Log($"보스가 {EnemyBossCount} 만큼 남았습니다");
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        StageEnd();
    }
    #endregion // 함수
}
