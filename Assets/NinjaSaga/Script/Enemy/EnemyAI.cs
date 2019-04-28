using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum RANGE
{
    ATTACKRANGE,
    CLOSERANGE,
    MIDRANGE,
    FARRANGE,
}
public enum ENEMYTACTIC
{
    ENGAGE=0,
    KEEPCLOSEDISTANCE=1,
    KEEPMEDIUMDISTANCE=2,
    KEEPFARDISTANCE=3,
    STANDSTILL=4,
}
public class EnemyAI : MonoBehaviour {
    [Space(10)]
    public bool enableAI;

    private List<UNITSTATE> ActiveAIStates = new List<UNITSTATE>
    {
        UNITSTATE.IDLE,
        UNITSTATE.WALK
    };

    private void Start()
    {
        
    }
}
