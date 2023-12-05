using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraManager : MonoBehaviour
{
    #region 변수
    [SerializeField] private Transform FollowTarget;
    [SerializeField] private Transform CameraArm;

    [SerializeField] private float Speed;
    [SerializeField] private float Senstivity;
    #endregion // 변수

    #region 함수
    /** 초기화 => 상태를 갱신한다 */
    private void Update()
    {
        LookAround();
        FollowCam();    
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Senstivity;
        Vector3 camAngle = this.transform.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 325f, 361f);
        }

        this.transform.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    private void FollowCam()
    {
        transform.position = Vector3.MoveTowards(transform.position, FollowTarget.position, Speed * Time.deltaTime);
    }
    #endregion // 함수
}
