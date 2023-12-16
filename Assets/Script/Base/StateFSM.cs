using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyStateFSM
{
    public abstract class StateFSM
    {
        public abstract void EnemyStateEnter(Enemy Enemy);
        public abstract void EnemyStateUpdate(Enemy Enemy, float Time);
        public abstract void EnemyStateExit(Enemy Enemy);
    }
}
