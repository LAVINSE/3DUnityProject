using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region 변수
    [SerializeField] private GameObject InventoryBase;
    [SerializeField] private GameObject SlotParent;

    private InventorySlot[] SlotArray;

    public static bool InventoryActivated = true;
    #endregion // 변수

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        SlotArray = SlotParent.GetComponentsInChildren<InventorySlot>();
    }

    /** 인벤토리 활성화 */
    public void OpenInventory() => InventoryBase.SetActive(true);

    /** 인벤토리 비활성화 */
    public void CloseInventory() => InventoryBase.SetActive(false);

    /** 아이템을 습득한다 */
    public void AcquireItem(ItemData Item, int Count = 1)
    {
        // 아이템 타입에 따라 세팅 if
        for(int i = 0; i < SlotArray.Length; i++)
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
    #endregion // 함수
}
