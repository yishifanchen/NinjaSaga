using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitState : MonoBehaviour
{
    public UNITSTATE currentState = UNITSTATE.IDLE;
    public void SetState(UNITSTATE state)
    {
        currentState = state;
    }
}
public enum UNITSTATE
{
    IDLE,
    WALK,
    RUN,
    JUMPING,
    LAND,
    GENERALATTACK,//玩家普通攻击
    ATTACK,//敌人攻击
    HEAVYBLOW,
    DEATH,
    DEFEND,
    SKILL1,
    SKILL2,
    SKILL3,
    SKILL4,
    HIT,
}
