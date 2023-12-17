using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopUI : MonoBehaviour
{
    #region 변수
    [Header("=====> 상점 설정 <=====")]
    [SerializeField] private Button ItemButtonA;
    [SerializeField] private Button ItemButtonB;
    [SerializeField] private Button ItemButtonC;

    [Space]
    [SerializeField] private Image ItemImgA;
    [SerializeField] private Image ItemImgB;
    [SerializeField] private Image ItemImgC;

    [Space]
    [SerializeField] private TMP_Text ItemNameAText;
    [SerializeField] private TMP_Text ItemNameBText;
    [SerializeField] private TMP_Text ItemNameCText;

    [Space]
    [SerializeField] private TMP_Text ItemPriceAText;
    [SerializeField] private TMP_Text ItemPriceBText;
    [SerializeField] private TMP_Text ItemPriceCText;

    [Space]
    [SerializeField] private Button ExitButton;

    private Shop MainShop;
    private ItemShopTable ItemData;
    #endregion // 변수

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        ExitButton.onClick.AddListener(() => this.gameObject.SetActive(false));
        
    }

    /** 상점 UI를 세팅한다 */
    public void ShopSetting(Shop MainShop,ItemShopTable ItemData)
    {
        this.MainShop = MainShop;
        this.ItemData = ItemData;

        // 이미지 설정
        ItemImgA.sprite = this.ItemData.ItemArray[0].ItemImg;
        ItemImgB.sprite = this.ItemData.ItemArray[1].ItemImg;
        ItemImgC.sprite = this.ItemData.ItemArray[2].ItemImg;

        // 아이템 이름 설정
        ItemNameAText.text = this.ItemData.ItemArray[0].ItemName;
        ItemNameBText.text = this.ItemData.ItemArray[1].ItemName;
        ItemNameCText.text = this.ItemData.ItemArray[2].ItemName;

        // 아이템 가격 설정
        ItemPriceAText.text = this.ItemData.ItemArray[0].ItemPrice.ToString();
        ItemPriceBText.text = this.ItemData.ItemArray[1].ItemPrice.ToString();
        ItemPriceCText.text = this.ItemData.ItemArray[2].ItemPrice.ToString();

        // 아이템 버튼 설정
        ItemButtonA.onClick.AddListener(() => MainShop.Buy(this.ItemData.ItemArray[0].ItemPrefab,
            this.ItemData.ItemArray[0].ItemName, this.ItemData.ItemArray[0].ItemPrice));
        ItemButtonB.onClick.AddListener(() => MainShop.Buy(this.ItemData.ItemArray[1].ItemPrefab,
            this.ItemData.ItemArray[1].ItemName, this.ItemData.ItemArray[1].ItemPrice));
        ItemButtonC.onClick.AddListener(() => MainShop.Buy(this.ItemData.ItemArray[2].ItemPrefab,
            this.ItemData.ItemArray[2].ItemName, this.ItemData.ItemArray[2].ItemPrice));
    }
    #endregion // 함수
}
