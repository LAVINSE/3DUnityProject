using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Ammo = 0,
        Coin,
        Grenade,
        Heart,
        Weapon,
    }

    #region 변수
    [SerializeField] private ItemType Type;
    [SerializeField] private int WeaponIndex;
    [SerializeField] private int ItemValue;

    private PlayerAction Player;
    private Rigidbody ItemRigid;
    private SphereCollider ItemSphereCollider;
    #endregion // 변수

    #region 프로퍼티
    public int oWeaponIndex
    {
        get => WeaponIndex;
        set => WeaponIndex = value;
    }
    public ItemType oType
    {
        get => Type;
        set => Type = value;
    }
    public int oItemValue
    {
        get => ItemValue;
        set => ItemValue = value;
    }
    #endregion // 프로퍼티

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        ItemRigid = GetComponent<Rigidbody>();

        // 첫번째 콜라이더
        ItemSphereCollider = GetComponent<SphereCollider>();
    }
    /** 초기화 => 상태를 갱신한다 */
    private void Update()
    {
        this.transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    /** 초기화 => 접촉했을 경우 */
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            ItemRigid.isKinematic = true;
            ItemSphereCollider.enabled = false;
        }
    }

    /** 초기화 => 접촉했을 경우 (트리거) */
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player = other.gameObject.GetComponent<PlayerAction>();

            if(Player != null)
            {
                Player.oItem = this;
            }
        }
    }

    /** 초기화 => 접촉이 끝났을 경우 (트리거) */
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(Player != null)
            {
                Player.oItem = null;
                Player = null;
            }
        }
    }
    #endregion // 함수
}
