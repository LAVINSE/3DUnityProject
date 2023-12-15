using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CFactory
{
    #region 클래스 함수
    // 원본 객체를 생성한다
    public static GameObject CreateObject(string Name, GameObject ParentObject,
        Vector3 Pos, Vector3 Scale, Vector3 Rotate, bool WorldPositionStays = false)
    {
        var Gameobj = new GameObject(Name);
        Gameobj.transform.SetParent(ParentObject?.transform, WorldPositionStays);

        Gameobj.transform.localPosition = Pos;
        Gameobj.transform.localScale = Scale;
        Gameobj.transform.localEulerAngles = Rotate;

        return Gameobj;
    }

    // 사본 객체를 생성한다
    public static GameObject CreateCloneObj(string Name, GameObject OriginObject, GameObject ParentObject,
        Vector3 Pos, Vector3 Scale, Vector3 Rotate, bool WorldPositionStays = false)
    {
        var Gameobj = GameObject.Instantiate(OriginObject, Vector3.zero, Quaternion.identity);
        Gameobj.name = Name;
        Gameobj.transform.SetParent(ParentObject?.transform, WorldPositionStays);

        Gameobj.transform.localPosition = Pos;
        Gameobj.transform.localScale = Scale;
        Gameobj.transform.localEulerAngles = Rotate;

        return Gameobj;
    }
    #endregion // 클래스 함수

    #region 제네릭 함수
    // 원본 객체를 생성한다
    public static T CreateObject<T>(string Name, GameObject ParentObject,
        Vector3 Pos, Vector3 Scale, Vector3 Rotate, bool WorldPositionStays = false) where T : Component
    {
        var Gameobj = CFactory.CreateObject(Name, ParentObject, Pos, Scale, Rotate, WorldPositionStays);

        // new로 생성되었기 때문에 컴포넌트가 없다
        return Gameobj.GetComponent<T>() ?? Gameobj.AddComponent<T>();
    }

    // 사본 객체를 생성한다
    public static T CreateCloneObj<T>(string Name, GameObject OriginObject, GameObject ParentObject,
        Vector3 Pos, Vector3 Scale, Vector3 Rotate, bool WorldPositionStays = false) where T : Component
    {
        var Gameobj = CFactory.CreateCloneObj(Name, OriginObject, ParentObject, Pos, Scale, Rotate, WorldPositionStays);

        return Gameobj.GetComponent<T>() ?? Gameobj.AddComponent<T>();
    }
    #endregion // 제네릭 함수
}
