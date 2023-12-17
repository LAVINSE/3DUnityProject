using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    #region 변수
    [SerializeField] private Image ItemImg;

    public InventorySlot InventoryDragSlot;

    public static DragSlot Instance;
    #endregion // 변수

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        Instance = this;
    }

    /** 드래그중인 아이템 이미지 세팅 */
    public void DragSetImg(Image ItemImg)
    {
        this.ItemImg.sprite = ItemImg.sprite;
        SetColor(1);
    }

    /** 아이템 이미지 투명도 조절 */
    public void SetColor(float Alpha)
    {
        Color color = ItemImg.color;
        color.a = Alpha;

        ItemImg.color = color;
    }
    #endregion // 함수
}
