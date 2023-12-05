using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    #region 변수
    [SerializeField] private GameObject GrenadeMeshObject;
    [SerializeField] private GameObject GrenadeEffectObject;
    [SerializeField] private Rigidbody GrenadeRigid;
    #endregion // 변수

    #region 함수
    private void Start()
    {
        // 수류탄 생성되고 작동
        StartCoroutine(GrenadeExplosion());
    }

    /** 수류탄 폭발 */
    private IEnumerator GrenadeExplosion()
    {
        // 3초뒤 폭발
        yield return new WaitForSeconds(3f);

        // 수류탄 회전, 이동 없애기
        GrenadeRigid.velocity = Vector3.zero;
        GrenadeRigid.angularVelocity = Vector3.zero;

        // 이펙트 활성화, 수류탄 비활성화
        GrenadeMeshObject.SetActive(false);
        GrenadeEffectObject.SetActive(true);

        // 구체모양의 레이캐스트 (모든오브젝트)
        RaycastHit[] RayHitArray = Physics.SphereCastAll(this.transform.position, 15f, Vector3.up, 0f,
            LayerMask.GetMask("Enemy"));

        // 레이캐스트에 담긴 모든 오브젝트에 피격 적용
        foreach(RaycastHit HitObject in RayHitArray)
        {
            // 수류탄 피격
            HitObject.transform.GetComponent<Enemy>().HitGrenade(this.transform.position);
        }

        Destroy(this.gameObject, 5);
    }
    #endregion // 함수
}
