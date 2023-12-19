using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPoolList<T>
{
    public List<T> oList = new List<T>(); // 활성화 객체
    public Queue<T> oQueue = new Queue<T>(); // 비활성화 객체
}

public class ObjectPoolManager : MonoBehaviour
{
    #region 변수
    private Dictionary<System.Type, CPoolList<object>> oPoolListDict = new Dictionary<System.Type, CPoolList<object>>();
    #endregion // 변수


    #region 함수
    /** 객체를 활성화 한다 */
    public object SpawnObj<T>(System.Func<object> Create)
    {
        var PoolList = oPoolListDict.GetValueOrDefault(typeof(T)) ?? new CPoolList<object>();
        // Queue에 비활성 객체가 하나이상 있을때 Dequeue 데이터 출력(비활성화 객체 가져오기) , Queue에 없으면 Creator에서 생성
        // Creator() 괄호 부분은 함수를 호출한다라는 뜻이다
        var Obj = (PoolList.oQueue.Count >= 1) ? PoolList.oQueue.Dequeue() : Create();

        // 중복 객체가 아닐 경우
        if (!PoolList.oList.Contains(Obj))
        {
            PoolList.oList.Add(Obj);
        }

        oPoolListDict.TryAdd(typeof(T), PoolList);

        return Obj;
    }

    /** 객체를 비활성화 한다 */
    public void DeSpawnObj<T>(object Objs, System.Action<object> Callback)
    {
        // 객체 풀이 존재 할 경우
        // Key에 해당하는 Value 값이 있으면 true 반환 그 값을 out으로 전달된 PoolList에 세팅 >> out은 참조
        if (oPoolListDict.TryGetValue(typeof(T), out CPoolList<object> PoolList))
        {
            PoolList.oList.Remove(Objs); // List에서 제거
            PoolList.oQueue.Enqueue(Objs); // Queue에 추가

            Callback?.Invoke(Objs);
        }
    }

    #endregion // 함수
}
