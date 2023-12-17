using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Ammo = 0,
        Coin,
        Grenade,
        Heart,
        Weapon,
    }

    public ItemType Type;
    public int ItemValue;
    public string ItemName;
    public Sprite ItemImg;
}
