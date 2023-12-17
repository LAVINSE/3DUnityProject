using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    #region 변수
    [Header("=====> 상점 데이터 <=====")]
    [SerializeField] private ItemShopTable ItemTableData;

    [Header("=====> 상점 오브젝트 <=====")]
    [SerializeField] private GameObject ItemShopPrefab;
    [SerializeField] private GameObject ItemShopRoot;
    [SerializeField] private GameObject ItemSpawnPos;

    [Header("=====> 상점 UI 설정 <=====")]
    [SerializeField] private Animator NpcAnimator;
    [SerializeField] private TMP_Text TalkText;
    [SerializeField] private string[] TalkDataArray;

    private PlayerAction Player;
    private GameObject ItemShopObject;
    #endregion // 변수

    #region 함수
    /** 초기화 => 접촉했을 경우 (트리거) */
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player = other.GetComponent<PlayerAction>();
            Player.oNearObject = this.gameObject;
        }
    }

    /** 초기화 => 접촉이 끝났을 경우 (트리거) */
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(Player != null)
            {
                // 상점 나가기
                Exit();
                Player.IsShop = false;
                Player.oNearObject = null;
                Player = null;
            }
        }
    }

    /** 상점에 입장한다 */
    public void Enter()
    {
        // 화면 중앙에 위치
        var ItemShop = this.transform.GetComponentInChildren<ItemShopUI>(true);

        // 아이템 상점UI가 없을경우
        if(ItemShop == null)
        {
            Debug.Log("입장");
            // 상점 생성, 설정
            ItemShopObject = CFactory.CreateCloneObj("ItemShop", ItemShopPrefab,
                ItemShopRoot, Vector3.zero, Vector3.one, Vector3.zero);
            ItemShopObject.GetComponent<ItemShopUI>().ShopSetting(this, ItemTableData);
            ItemShopObject.SetActive(true);
        }
        else
        {
            ItemShopObject.SetActive(true);
        }
    }

    /** 상점을 나간다 */
    public void Exit()
    {
        NpcAnimator.SetTrigger("TriggerHello");

        // 상점 UI 숨기기
        if(ItemShopObject != null)
        {
            ItemShopObject.SetActive(false);
        }
    }

    /** 물건을 구입한다 */
    public void Buy(GameObject ItemPrefab, string ItemName, int ItemPrice)
    {
        // 아이템 가격이 플레이어 돈 보다 높을경우
        if(ItemPrice > Player.oCoin)
        {
            //StopCoroutine(Talk());
            //StartCoroutine(Talk());
            return;
        }

        // 아이템이 생성되어있지 않을경우
        if(ItemSpawnPos.gameObject.transform.childCount == 0)
        {
            // 아이템 가격만큼 차감
            Player.oCoin -= ItemPrice;

            // 아이템 생성
            CFactory.CreateCloneObj(ItemName, ItemPrefab, ItemSpawnPos,
                Vector3.zero, Vector3.one, Vector3.zero);
        }
        else
        {
            Debug.Log("아이템 있음");
        }
        
    }

    /** Npc 대화 */
    private IEnumerator Talk()
    {
        // 돈이 없다고 알림
        TalkText.text = TalkDataArray[1];
        yield return new WaitForSeconds(2f);
        TalkText.text = TalkDataArray[0];
    }
    #endregion // 함수
}
