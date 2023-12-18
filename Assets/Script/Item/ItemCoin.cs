using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCoin : Item
{
    #region 변수
    #endregion // 변수

    #region 함수
    /** 초기화 */
    public override void Awake()
    {
        base.Awake();

        ItemValue = ItemDataTable.ItemValue;
        ItemName = ItemDataTable.ItemName;
        ItemImg = ItemDataTable.ItemImg;

        oItemDataTable.Use = ItemUse;
    }

    /** 아이템을 사용한다 */
    public override void ItemUse()
    {
        base.ItemUse();
        Player.oCoin += ItemValue;
    }
    #endregion // 함수
}
