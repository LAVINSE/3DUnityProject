using EnemyStateFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
    public enum EnemyStateType
    {
        Wait,
        Tracking,
        Attack,
        Hit,
        Dead,
    }

    #region 변수
    private StateFSM[] StateArray;
    private StateFSM CurrentState;
    private Enemy Enemy;
    public Enemy OtherEnemy;
    #endregion // 변수

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        Enemy = GetComponent<Enemy>();
        
        // 상태 저장공간
        StateArray = new StateFSM[10];

        // 초기 상태
        StateArray[(int)EnemyStateType.Wait] = new EnemyStateWait();
        StateArray[(int)EnemyStateType.Tracking] = new EnemyStateTracking();
        CurrentState = StateArray[(int)EnemyStateType.Wait];
        CurrentState.State = this;
    }

    /** 초기화 => 상태를 갱신한다 */
    private void Update()
    {
        // 상태가 존재할 경우
        if(CurrentState != null)
        {
            CurrentState.EnemyStateUpdate(this.Enemy, Time.deltaTime);
        }
    }

    /** 상태를 변경한다 */
    public void ChangeState(EnemyStateType NewState)
    {
        if (StateArray[(int)NewState] == null) { return; }

        if(CurrentState != null)
        {
            CurrentState.EnemyStateExit(this.Enemy);
        }

        CurrentState = StateArray[(int)NewState];
        CurrentState.EnemyStateEnter(this.Enemy);
    }
    #endregion // 함수

    #region 상태 클래스
    /** 대기 */
    public class EnemyStateWait : StateFSM
    {
        public override void EnemyStateEnter(Enemy Enemy)
        {
            
        }

        public override void EnemyStateUpdate(Enemy Enemy, float Time)
        {
            Debug.Log("찾는중");
            var Distance = Enemy.oTarget.transform.position - Enemy.transform.position;

            if (Distance.magnitude.ExIsLessEquals(Enemy.oTrackingRange))
            {
                Debug.Log("추적");
                State.ChangeState(EnemyStateType.Tracking);
            }
            else if (Distance.magnitude.ExIsLessEquals(Enemy.oAttackRange))
            {
                Debug.Log("공격");
                //State.ChangeState(EnemyStateType.Attack);
            }
        }

        public override void EnemyStateExit(Enemy Enemy)
        {
            
        }
    }

    /** 추적 */
    public class EnemyStateTracking : StateFSM
    {
        public override void EnemyStateEnter(Enemy Enemy)
        {
            Debug.Log("추적 시작");
            // 추적 시작
            Enemy.TrackingStart();
        }

        public override void EnemyStateUpdate(Enemy Enemy, float Time)
        {
            Debug.Log("추적 중");

            // 추적 중
            Enemy.Tracking();
        }

        public override void EnemyStateExit(Enemy Enemy)
        {

        }
    }

    /** 공격 */
    public class EnemyStateAttack : StateFSM
    {
        public override void EnemyStateEnter(Enemy Enemy)
        {
            
        }

        public override void EnemyStateUpdate(Enemy Enemy, float Time)
        {
            
        }

        public override void EnemyStateExit(Enemy Enemy)
        {

        }
    }
    #endregion // 상태 클래스
}
