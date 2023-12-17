using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler,
    IDragHandler, IEndDragHandler, IDropHandler
{
    #region 변수
    [SerializeField] public ItemData PlusItem; // 획득한 아이템
    [SerializeField] public int ItemCount; // 획득한 아이템 수
    [SerializeField] public Image ItemImg; // 아이템 이미지
    [SerializeField] private GameObject ItemCountImg;
    [SerializeField] private TMP_Text ItemCountText;
    
    #endregion // 변수

    #region 함수
    /** 아이템 이미지 투명도 조절 */
    private void SetColor(float Alpha)
    {
        Color color = ItemImg.color;
        color.a = Alpha;
        ItemImg.color = color;
    }

    /** 아이템 추가 */
    public void AddItem(ItemData Item, int Count = 1)
    {
        this.PlusItem = Item;
        ItemCount = Count;
        ItemImg.sprite = Item.ItemImg;

        // 아이템 타입에 따라 관리 if
        ItemCountImg.SetActive(true);
        ItemCountText.text = ItemCount.ToString();

        // 투명도
        SetColor(1);
    }

    /** 아이템 개수 조정 */
    public void SetSlotCount(int Count)
    {
        ItemCount += Count;
        ItemCountText.text = ItemCount.ToString();

        // 아이템이 없을경우
        if(ItemCount <= 0)
        {
            // 슬롯 초기화
            ClearSlot();
        }
    }

    /** 슬롯을 교환한다 */
    private void ChangeSlot()
    {
        // 데이터 덮어쓰기전 저장
        ItemData ItemTemp = PlusItem;
        int ItemCountTemp = ItemCount;

        // 아이템 추가
        AddItem(DragSlot.Instance.InventoryDragSlot.PlusItem, DragSlot.Instance.InventoryDragSlot.ItemCount);

        // 슬롯 교환
        if (ItemTemp != null)
        {
            DragSlot.Instance.InventoryDragSlot.AddItem(ItemTemp, ItemCountTemp);
        }
        else
        {
            DragSlot.Instance.InventoryDragSlot.ClearSlot();
        }
    }

    /** 슬롯 초기화 */
    private void ClearSlot()
    {
        // 초기화
        PlusItem = null;
        ItemCount = 0;
        ItemImg.sprite = null;

        // 투명도
        SetColor(0);

        ItemCountText.text = string.Empty;
        ItemCountImg.SetActive(false);
    }

    /** 클릭했을때 */
    public void OnPointerClick(PointerEventData eventData)
    {
        // 오른쪽 클릭했을경우
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            // 아이템이 있을경우
            if(PlusItem != null)
            {
                // 아이템 타입에 따라 장착 , 사용 여부 if
                
                Debug.Log(PlusItem.ItemName + "를 사용했습니다");

                // 개수 조정
                SetSlotCount(-1);
            }
        }
    }

    /** 드래그를 시작했을때 */
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 아이템이 있을경우
        if (PlusItem != null)
        {
            // 드래그중인 아이템 세팅
            DragSlot.Instance.InventoryDragSlot = this;
            DragSlot.Instance.DragSetImg(ItemImg);

            DragSlot.Instance.transform.position = eventData.position;
        }
    }

    /** 드래그 중일때 */
    public void OnDrag(PointerEventData eventData)
    {
        // 아이템이 있을경우
        if (PlusItem != null)
        {
            DragSlot.Instance.transform.position = eventData.position;
        }
    }

    /** 드래그가 끝났을때 */
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");

        // 투명도, 비우기
        DragSlot.Instance.SetColor(0);
        DragSlot.Instance.InventoryDragSlot = null;
    }

    /** 드랍했을때 */
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop");

        // 드래그중인 아이템이 있을경우
        if (DragSlot.Instance.InventoryDragSlot != null)
        {
            // 아이템 교환
            ChangeSlot();
        }
    }
    #endregion // 함수
}
