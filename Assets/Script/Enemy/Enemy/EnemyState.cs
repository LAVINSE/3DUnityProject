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
        StateArray[(int)EnemyStateType.Attack] = new EnemyStateAttack();
        CurrentState = StateArray[(int)EnemyStateType.Wait];
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
            Debug.Log("대기");
        }

        public override void EnemyStateUpdate(Enemy Enemy, float Time)
        {
            Debug.Log("찾는중");

            // 플레이어와 적 거리
            var PlayerDistance = Enemy.oPlayerTarget.transform.position - Enemy.transform.position;
            // 석상과 적 거리
            var StoneStatueDistance = Enemy.oStoneStatueTarget.transform.position - Enemy.transform.position;

            // 추적 범위안에 있을경우
            if (StoneStatueDistance.magnitude.ExIsGreat(Enemy.oTrackingRange) ||
                PlayerDistance.magnitude.ExIsLessEquals(Enemy.oTrackingRange))
            {
                Debug.Log("추적상태");
                Enemy.EnemyStateMachine.ChangeState(EnemyStateType.Tracking);
            }
            // 공격 범위안에 있을경우
            else if (PlayerDistance.magnitude <= Enemy.oAttackRange)
            {
                Debug.Log("공격상태");
                Enemy.EnemyStateMachine.ChangeState(EnemyStateType.Attack);
            }
        }

        public override void EnemyStateExit(Enemy Enemy)
        {
            Debug.Log("대기 종료");
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

            // 플레이어와 적 거리
            var PlayerDistance = Enemy.oPlayerTarget.transform.position - Enemy.transform.position;
            // 석상과 적 거리
            var StoneStatueDistance = Enemy.oStoneStatueTarget.transform.position - Enemy.transform.position;

            // 석상 추적
            if (StoneStatueDistance.magnitude.ExIsGreat(Enemy.oTrackingRange))
            {
                // 추적 중
                Enemy.Tracking(Enemy.oStoneStatueTarget);

                // 추적 범위안에 있을경우
                if (PlayerDistance.magnitude.ExIsLessEquals(Enemy.oTrackingRange) &&
                    PlayerDistance.magnitude.ExIsGreat(Enemy.oAttackRange))
                {
                    // 추적 중
                    Enemy.Tracking(Enemy.oPlayerTarget);
                }
                // 공격 범위 안에 있을경우
                else if (PlayerDistance.magnitude.ExIsLessEquals(Enemy.oAttackRange))
                {
                    Enemy.EnemyStateMachine.ChangeState(EnemyStateType.Attack);
                }
            }
            
        }

        public override void EnemyStateExit(Enemy Enemy)
        {
            Debug.Log("추적 종료");
        }
    }

    /** 공격 */
    public class EnemyStateAttack : StateFSM
    {
        public override void EnemyStateEnter(Enemy Enemy)
        {
            Debug.Log("공격 시작");
        }

        public override void EnemyStateUpdate(Enemy Enemy, float Time)
        {
            Debug.Log("공격상태");
            var PlayerDistance = Enemy.oPlayerTarget.transform.position - Enemy.transform.position;

            // 공격범위 안에 있을경우
            if (PlayerDistance.magnitude.ExIsLessEquals(Enemy.oAttackRange))
            {
                Enemy.Targeting();
            }
            // 공격범위 밖에 있을경우, 추적준비 완료일 경우
            else if(PlayerDistance.magnitude.ExIsGreat(Enemy.oAttackRange) && Enemy.IsTracking)
            {
                Enemy.EnemyStateMachine.ChangeState(EnemyStateType.Tracking);
            }
            
        }

        public override void EnemyStateExit(Enemy Enemy)
        {
            Debug.Log("공격 종료");
        }
    }
    #endregion // 상태 클래스
}
