using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventorySlotToolTip : MonoBehaviour
{
    #region 변수
    [SerializeField] private GameObject ToolTip;
    [SerializeField] private TMP_Text ItemNameText;
    [SerializeField] private TMP_Text ItemDescText;
    [SerializeField] private TMP_Text ItemDescUseText;
    #endregion // 변수

    #region 함수
    /** 툴팁을 보여준다 */
    public void ShowToolTip(ItemData Item)
    {
        ToolTip.SetActive(true);

        ItemNameText.text = Item.ItemName;
        ItemDescText.text = Item.ItemDesc;
        ItemDescUseText.text = Item.ItemDescUse;
    }

    /** 툴팁을 숨긴다 */
    public void HideToolTip()
    {
        ToolTip.SetActive(false);
    }
    #endregion // 함수
}
