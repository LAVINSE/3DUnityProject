using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemDropTable : ScriptableObject
{
    [System.Serializable]
    public class DropTable
    {
        public ItemData[] ItemArray;
        public float Weight;
    }

    #region 변수
    public float TotalWeight = 0;
    public List<DropTable> DropTableList = new List<DropTable>();
    #endregion // 변수

    #region 함수
    /** 아이템 드랍로직 */
    private ItemData[] PickItem()
    {
        var Rand = Mathf.Floor(TotalWeight * Random.Range(0.0f, 1.0f));
        float Percent = 0;

        for(int i =0; i < DropTableList.Count; i++)
        {
            Percent += DropTableList[i].Weight;

            if(Rand <= Percent)
            {
                return DropTableList[i].ItemArray;
            }
        }

        return null;
    }

    /** 아이템 드랍 */
    public ItemData[] ItemDrop()
    {
        var ItemPick = PickItem();

        if(ItemPick == null) { return null; }

        return ItemPick;
    }
    #endregion // 함수
}
