using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    #region 변수
    [SerializeField] private RectTransform ShopUiGroup;
    [SerializeField] private Animator NpcAnimator;
    [SerializeField] private GameObject[] ItemPrefabArray;
    [SerializeField] private int[] ItemPriceArray;
    [SerializeField] private Transform[] ItemPosArray;
    [SerializeField] private TMP_Text TalkText;
    [SerializeField] private string[] TalkDataArray;

    private PlayerAction Player;
    #endregion // 변수

    #region 함수
    /** 상점에 입장한다 */
    public void Enter(PlayerAction Player)
    {
        this.Player = Player;

        // 화면 중앙에 위치
        ShopUiGroup.anchoredPosition = Vector3.zero;
    }

    /** 상점을 나간다 */
    public void Exit()
    {
        NpcAnimator.SetTrigger("TriggerHello");
        // 아래로 숨기기
        ShopUiGroup.anchoredPosition = Vector3.down * 1000;
    }

    /** 물건을 구입한다 */
    public void Buy(int Index)
    {
        int ItemPrice = ItemPriceArray[Index];
        if(ItemPrice > Player.oCoin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        // 아이템 가격만큼 차감
        Player.oCoin -= ItemPrice;

        // 아이템 소환랜덤위치
        Vector3 RandomPos = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3);

        // 아이템 생성
        Instantiate(ItemPrefabArray[Index], ItemPosArray[Index].position + RandomPos,
            ItemPosArray[Index].rotation);
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
