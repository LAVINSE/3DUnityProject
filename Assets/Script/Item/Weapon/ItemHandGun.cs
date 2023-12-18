using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandGun : Item
{
    #region 변수
    #endregion // 변수

    #region 함수
    /** 초기화 */
    public override void Awake()
    {
        base.Awake();

        WeaponIndex = ItemDataTable.WeaponIndex;
        ItemName = ItemDataTable.ItemName;
        ItemImg = ItemDataTable.ItemImg;

        oItemDataTable.Use = ItemUse;
    }

    /** 아이템을 사용한다 */
    public override void ItemUse()
    {
        base.ItemUse();
        if (Player.oHasWeaponArray[WeaponIndex] == true)
        {
            Debug.Log("이미 장착중인 아이템 입니다");
        }
        else
        {
            // 무기 인덱스 번호를 가져온다
            Player.oHasWeaponArray[WeaponIndex] = true;
        }
    }
    #endregion // 함수
}
