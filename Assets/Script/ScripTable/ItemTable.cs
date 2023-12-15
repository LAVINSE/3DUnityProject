using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class ItemTable : ScriptableObject
{
    [System.Serializable]
    public struct ItemInfo
    {
        public Sprite ItemImg;
        public string ItemName;
        public int ItemPrice;
        public GameObject ItemPrefab;
    }

    public ItemInfo[] ItemArray;
}
