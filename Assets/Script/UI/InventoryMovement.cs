using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryMovement : MonoBehaviour, IPointerDownHandler, IDragHandler
{

    #region 변수
    [SerializeField] private Transform TargetTransform;

    private Vector2 BeginPoint;
    private Vector2 MoveBegin;
    #endregion // 변수

    #region 함수
    // 드래그 시작 위치
    public void OnPointerDown(PointerEventData eventData)
    {
        BeginPoint = TargetTransform.position;
        MoveBegin = eventData.position;
    }

    // 드래그
    public void OnDrag(PointerEventData eventData)
    {
        TargetTransform.position = BeginPoint + (eventData.position - MoveBegin);
    }
    #endregion // 함수
}
