using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    #region 변수
    [Header("=====> 인벤토리 설정 <=====")]
    [SerializeField] private GameObject InventoryObject;
    [SerializeField] private GameObject InventoryContent;
    [SerializeField] private Button InventoryCloseButton;

    public InventorySlot[] SlotArray;
    #endregion // 변수

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        SlotArray = InventoryContent.GetComponentsInChildren<InventorySlot>(true);
        InventoryCloseButton.onClick.AddListener(() => CloseButton());
    }

    /** 인벤토리 활성화 */
    public void OpenInventory() => InventoryObject.SetActive(true);

    /** 인벤토리 비활성화 */
    public void CloseInventory() => InventoryObject.SetActive(false);

    /** 아이템을 습득한다 */
    public void AcquireItem(ItemData Item, int Count = 1)
    {
        // 아이템 타입에 따라 세팅 if
        for (int i = 0; i < SlotArray.Length; i++)
        {
            if (SlotArray[i].PlusItem != null)
            {
                if (SlotArray[i].PlusItem.ItemName == Item.ItemName)
                {
                    SlotArray[i].SetSlotCount(Count);
                    return;
                }
            }
        }

        for (int i = 0; i < SlotArray.Length; i++)
        {
            if (SlotArray[i].PlusItem == null)
            {
                SlotArray[i].AddItem(Item, Count);
                return;
            }
        }
    }

    /** 인벤토리 닫기 버튼을 누른다 */
    private void CloseButton()
    {
        GameManager.Inst.CursorLock();
        this.gameObject.SetActive(false);
    }
    #endregion // 함수
}
