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
    public float knockbackForce = 4;//the horizontal force of a knockdown
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
    private void Start()
    {
        OnStart();
    }
    private void FixedUpdate()
    {
        OnFixedUpdate();
    }
    void OnFixedUpdate()
    {
        if (updateVelocity)
        {
            rb.velocity = fixedVelocity;
            updateVelocity = false;
        }
    }
    void SetVelocity(Vector3 velocity)
    {
        fixedVelocity = velocity;
        updateVelocity = true;
    }
    //we are hit  被攻击
    public void Hit(DamageObject d)
    {
        //Camera Shake
        CamShake camShake = Camera.main.GetComponent<CamShake>();
        if (camShake != null)
            camShake.Shake(.2f);

        //check for hit
        anim.SetAnimatorTrigger("Hit1");
        enemyState = UNITSTATE.HIT;

        //add small force from the impact
        LookAtTarget(d.inflictor.transform);
        anim.AddForce(-knockbackForce);
    }
    public void OnStart()
    {
        if (target == null) target = GameObject.FindGameObjectWithTag("Player");
    }
    public void WalkTo(float proximityRange,float movementMargin)
    {
        Vector3 dirToTarget;
        LookAtTarget(target.transform);
        enemyState = UNITSTATE.WALK;

        if (enemyTactic == ENEMYTACTIC.ENGAGE)
        {
            dirToTarget = target.transform.position - (transform.position + new Vector3(0, 0, Mathf.Clamp(ZSpread, 0, attackRangeDistance)));
        }
        else
        {
            dirToTarget = target.transform.position - (transform.position + new Vector3(0, 0, ZSpread));
        }
        //距离玩家太远了，靠近点
        if (distance >= proximityRange)
        {
            moveDirection = new Vector3(dirToTarget.x, 0, dirToTarget.z);
            if (IsGrounded())
            {
                Move(moveDirection.normalized,walkSpeed);
                anim.SetAnimatorFloat("MovementSpeed", rb.velocity.sqrMagnitude);
                return;
            }
        }
        //距离玩家太近了，走远点
        if (distance <= proximityRange - movementMargin)
        {
            moveDirection = new Vector3(-dirToTarget.x, 0, 0);
            if (IsGrounded())
            {
                Move(moveDirection.normalized, walkBackwardSpeed);
                anim.SetAnimatorFloat("MovementSpeed", -rb.velocity.sqrMagnitude);
                return;
            }
        }
    }
    public void Move(Vector3 vector,float speed)
    {
        if (!NoMovementStates.Contains(enemyState))
        {
            SetVelocity(new Vector3(vector.x * speed, rb.velocity.y + Physics.gravity.y * Time.fixedDeltaTime, vector.z * speed));
        }
        else
        {
            SetVelocity(Vector3.zero);
        }
    }
    //朝向当前的目标
    public void LookAtTarget(Transform _target)
    {
        if (_target != null)
        {
            Vector3 newDir = Vector3.zero;
            int dir = _target.transform.position.x >= transform.position.x ? 1 : -1;
            currentDirection = (DIRECTION)dir;
            if (anim != null) anim.currentDirection = currentDirection;
            newDir = Vector3.RotateTowards(transform.forward, Vector3.forward * dir, rotationSpeed * Time.deltaTime, 0);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }
    //是否接触地面
    public bool IsGrounded()
    {
        float colliderSize = capsule.bounds.extents.y - .1f;
        if (Physics.CheckCapsule(capsule.bounds.center, capsule.bounds.center + Vector3.down * colliderSize, capsule.radius, collisionLayer)){
            isGrounded = true;
            return true;
        }
        else
        {
            isGrounded = false;
            return false;
        }
    }
    /// <summary>
    /// turn towards a direction
    /// </summary>
    /// <param name="dir"></param>
    public void TurnToDir(DIRECTION dir)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward * (int)dir);
    }
    /// <summary>
    /// 攻击
    /// </summary>
    public void ATTACK()
    {
        //print("gongji ");
        var playerMovement = target.GetComponent<PlayerMovement>();
        if (!attackPlayerAirborne && playerMovement != null && playerMovement.jumpInProgress) return;
        else
        {
            enemyState = UNITSTATE.ATTACK;
            Move(Vector3.zero, 0f);
            LookAtTarget(target.transform);
            TurnToDir(currentDirection);

            if (pickRandomAttack) attackCounter = Random.Range(0, attackList.Length);

            anim.SetAnimatorTrigger(attackList[attackCounter].animTrigger);

            if (!pickRandomAttack)
            {
                attackCounter += 1;
                if (attackCounter >= attackList.Length) attackCounter = 0;
            }

            lastAttackTime = Time.time;
            lastAttack = attackList[attackCounter];

            Invoke("READY", attackList[attackCounter].duration);
        }
    }
    public void READY()
    {
        enemyState = UNITSTATE.IDLE;
        anim.SetAnimatorTrigger("Idle");
        anim.SetAnimatorFloat("MovementSpeed", 0);
        Move(Vector3.zero,0);
    }
    public void CheckForHit()
    {
        Vector3 boxPosition = transform.position + (Vector3.up * lastAttack.collHeight) + Vector3.right * ((int)currentDirection * lastAttack.collDistance);
        Vector3 boxSize = new Vector3(lastAttack.collSize / 2, lastAttack.collSize / 2, hitZRange / 2);
        Collider[] hitColliders = Physics.OverlapBox(boxPosition, boxSize, Quaternion.identity, hitLayerMask);
        int i = 0;
        while (i < hitColliders.Length)
        {
            IDamagable<DamageObject> damageObject = hitColliders[i].GetComponent(typeof(IDamagable<DamageObject>)) as IDamagable<DamageObject>;
            if (damageObject != null&&damageObject!=(IDamagable<DamageObject>)this)
            {
                damageObject.Hit(lastAttack);
            }
            i++;
        }
    }
}
