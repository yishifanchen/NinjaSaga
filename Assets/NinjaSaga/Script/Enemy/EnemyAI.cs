using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 范围
/// </summary>
public enum RANGE
{
    ATTACKRANGE,//攻击范围
    CLOSERANGE,//近距离
    MIDRANGE,//中等距离
    FARRANGE,//远距离
}
public enum ENEMYTACTIC
{
    ENGAGE=0,
    KEEPCLOSEDISTANCE=1,
    KEEPMEDIUMDISTANCE=2,
    KEEPFARDISTANCE=3,
    STANDSTILL=4,
}
public class EnemyAI : EnemyActions, IDamagable<DamageObject>{
    [Space(10)]
    public bool enableAI;

    private List<UNITSTATE> ActiveAIStates = new List<UNITSTATE>
    {
        UNITSTATE.IDLE,
        UNITSTATE.WALK
    };

    void Start()
    {
        OnStart();
    }
    void Update()
    {
        if (target == null || !enableAI)
        {
            READY();
            return;
        }
        else
        {
            range = GetDistanceToTarget();
        }
        if (!isDead && enableAI)
        {
            if (ActiveAIStates.Contains(enemyState) && targetSpotted)
            {
                AI();
            }
            else
            {
                //尝试寻找玩家
                if (distanceToTarget.magnitude < sightDistance) targetSpotted = true;
            }
        }
    }
    void AI()
    {
        LookAtTarget(target.transform);
        if (range == RANGE.ATTACKRANGE)
        {
            if (!cliffSpotted)
            {
                if (Time.time - lastAttackTime > attackInterval)
                {
                    ATTACK();
                }
                else
                {
                    READY();
                }
                return;
            }

            if (enemyTactic == ENEMYTACTIC.KEEPCLOSEDISTANCE) WalkTo(closeRangeDistance, 0);
            if (enemyTactic == ENEMYTACTIC.KEEPMEDIUMDISTANCE) WalkTo(midRangeDistance, rangeMarging);
            if (enemyTactic == ENEMYTACTIC.KEEPFARDISTANCE) WalkTo(farRangeDistance, rangeMarging);
            if (enemyTactic == ENEMYTACTIC.STANDSTILL) READY();
        }
        else
        {
            if (enemyTactic == ENEMYTACTIC.ENGAGE) WalkTo(attackRangeDistance, 0);
            if (enemyTactic == ENEMYTACTIC.KEEPCLOSEDISTANCE) WalkTo(closeRangeDistance, rangeMarging);
            if (enemyTactic == ENEMYTACTIC.KEEPMEDIUMDISTANCE) WalkTo(midRangeDistance, rangeMarging);
            if (enemyTactic == ENEMYTACTIC.KEEPFARDISTANCE) WalkTo(farRangeDistance, rangeMarging);
            if (enemyTactic == ENEMYTACTIC.STANDSTILL) READY();
        }
    }
    /// <summary>
    /// 获取距玩家的范围
    /// </summary>
    /// <returns></returns>
    private RANGE GetDistanceToTarget()
    {
        if (target != null)
        {
            distanceToTarget = target.transform.position - transform.position;
            distance = Vector3.Distance(target.transform.position,transform.position);

            float distX = Mathf.Abs(distanceToTarget.x);
            float distZ = Mathf.Abs(distanceToTarget.z);

            if (distX <= attackRangeDistance)
            {
                if (distZ < (hitZRange / 2))
                    return RANGE.ATTACKRANGE;
                else
                    return RANGE.CLOSERANGE;
            }
            if (distX > attackRangeDistance && distX < midRangeDistance) return RANGE.CLOSERANGE;
            if (distX > closeRangeDistance && distX < farRangeDistance) return RANGE.MIDRANGE;
            if (distX > farRangeDistance) return RANGE.FARRANGE;
        }
        return RANGE.FARRANGE;
    }
}
