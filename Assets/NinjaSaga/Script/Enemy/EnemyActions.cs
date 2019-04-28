using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class EnemyActions : MonoBehaviour {
    [Space(10)]
    [Header("Linked components")]
    public GameObject target;//current target
    public UnitAnimator anim;//anim component
    public GameObject GFX;//GFX of this unit
    public Rigidbody rb;//rigidbody component
    public CapsuleCollider capsule;

    [Header("Attack Data")]
    public DamageObject[] attackList;
    public bool pickRandomAttack;
    public float hitZRange = 2;
    public float defendChance = 0;
    public float hitRecoveryTime = .4f;
    public float standUpTime = 1.1f;
    public bool canDefendDuringAttack;
    public bool attackPlayerAirborne;
    private DamageObject lastAttack;
    private int attackCounter = 0;
    public bool canHitEnemies;
    public bool canHitDestroyableObjects;
    [HideInInspector]
    public float lastAttackTime;

    [Header("Settings")]
    public bool pickRandomName;
    public TextAsset enemyNamesList;
    public string enemyName = "";
    public float attackRangeDistance = 1.4f;
    public float closeRangeDistance = 2;
    public float midRangeDistance = 3;
    public float farRangeDistance = 4.5f;
    public float rangeMarging = 1;
    public float walkSpeed = 1.95f;
    public float walkBackwardSpeed = 1.2f;
    public float sightDistance = 10f;
    public float attackInterval = 1.2f;
    public float rotationSpeed = 15;
    public float lookaheadDistance;
    public bool ignoreCliffs;
    public float knockdownTimeout = 0;
    public float knockdownUpForce = 5;
    public float knockbackForce = 4;
    private LayerMask hitLayerMask;
    public LayerMask collisionLayer;
    public bool randomizeValues = true;
    [HideInInspector]
    public float zSpreadMultiplier = 2;

    [Header("Stats")]
    public RANGE range;
    public ENEMYTACTIC enemyTactic;
    public UNITSTATE enemyState;
    public DIRECTION currentDirection;
    public bool targetSpotted;
    public bool cliffSpotted;
    public bool wallSpotted;
    public bool isGrounded;
    public bool isDead;
    private Vector3 moveDirection;
    public float distance;
    private Vector3 fixedVelocity;
    private bool updateVelocity;

    private List<UNITSTATE> NoMovementStates = new List<UNITSTATE>
    {
        UNITSTATE.DEATH,
        UNITSTATE.GENERALATTACK,
        UNITSTATE.DEFEND,
        UNITSTATE.IDLE,
    };
    private List<UNITSTATE> HitableStates = new List<UNITSTATE>
    {
        UNITSTATE.GENERALATTACK,
        UNITSTATE.WALK,
        UNITSTATE.DEFEND,
        UNITSTATE.IDLE,
    };

    public float ZSpread;
    public Vector3 distanceToTarget;
    private List<UNITSTATE> defendableStates = new List<UNITSTATE> { UNITSTATE.IDLE, UNITSTATE.WALK, UNITSTATE.DEFEND };

    public delegate void UnitEventHandler(GameObject Unit);
    public static event UnitEventHandler OnUnitDestroy;
}
