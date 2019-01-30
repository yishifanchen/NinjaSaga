using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitState : MonoBehaviour {
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
    PUNCH,
    KICK,
    DEATH,
}
