using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Consumable,
        Weapon,
    }

    public ItemType Type;
    public int ItemValue;
    public int WeaponIndex;
    public string ItemName;
    public Sprite ItemImg;

    public Action Use;
}
