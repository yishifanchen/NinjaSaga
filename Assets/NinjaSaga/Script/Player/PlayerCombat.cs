using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitState))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : MonoBehaviour, IDamagable<DamageObject>
{
    [Header("Linked Components")]
    public Transform weaponBone;
    public Transform swordHandPos;
    private UnitAnimator animator;
    private UnitState playerState;
    private PlayerMovement playerMovement;
    private Rigidbody rb;

    [Header("Attack Date & Combos")]
    public float hitZRange = 2;//Z轴攻击范围
    private int attackNum = -1;//当前攻击组合编号

    [Space(5)]
    public DamageObject[] generalAttackCombo;//普攻连击数据
    public DamageObject heavyBlowData;//重击数据
    public DamageObject skill1Data;//技能1数据
    public DamageObject skill2Data;//技能2数据
    private DamageObject lastAttack;

    [Header("Settings")]
    public bool canTurnWhileDefending;//allow turning while defending

    [Header("Stats")]
    public DIRECTION currentDirection;
    private float lastAttackTime = 0;//time of the last attack
    private bool continueGeneralAttackCombo;//true if a generalattack combo need to continue;

    [SerializeField]
    private bool targetHit;
    private bool isGrounded;
    private int enemyLayer;
    private int destroyableObjectLayer;
    private LayerMask hitLayerMask;//a list of all hitable objects
    private Vector3 fixedVelocity;
    private bool updateVelocity;
    private InputManager inputManager;
    private INPUTACTION lastAttackInput;
    private DIRECTION lastAttackDirection;

    private void Start()
    {
        animator = GetComponentInChildren<UnitAnimator>();
        playerState = GetComponent<UnitState>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        inputManager = GameObject.FindObjectOfType<InputManager>();
        currentDirection = DIRECTION.Right;

        enemyLayer = LayerMask.NameToLayer("Enemy");
        destroyableObjectLayer = LayerMask.NameToLayer("DestroyableObjectLayer");
        hitLayerMask = (1 << enemyLayer | 1 << destroyableObjectLayer);


    }
    private void Update()
    {
        if (animator) isGrounded = animator.animator.GetBool("isGrounded");
        if (isGrounded) Defend(inputManager.IsDefendKeyDown());
    }
    private void FixedUpdate()
    {
        if (updateVelocity)
        {
            rb.velocity = fixedVelocity;
            updateVelocity = false;
        }
    }
    /// <summary>
    /// 移动输入事件
    /// </summary>
    /// <param name="inputVector"></param>
    private void MovementInputEvent(Vector2 inputVector)
    {
        int dir = Mathf.RoundToInt(Mathf.Sign((float)-inputVector.x));
        if (Mathf.Abs(inputVector.x) > 0) currentDirection = (DIRECTION)dir;
    }
    /// <summary>
    /// 战斗输入事件
    /// </summary>
    /// <param name="action"></param>
    private void CombatInputEvent(INPUTACTION action)
    {
        if (action == INPUTACTION.SKILL1 && isGrounded)
        {
            DoAttack(skill1Data, UNITSTATE.SKILL1, INPUTACTION.SKILL1);
        }
        if (action == INPUTACTION.SKILL2 && isGrounded)
        {
            DoAttack(skill2Data, UNITSTATE.SKILL2, INPUTACTION.SKILL2);
        }
        //普攻
        if (action == INPUTACTION.GENERALATTACK && playerState.currentState != UNITSTATE.GENERALATTACK && isGrounded)
        {
            //continue to the next attack if the time is inside the combo window   最后一次攻击时间，最后一次攻击的持续时间，最后一次攻击连击的重置时间
            bool insideComboWindow = (lastAttack != null && Time.time < (lastAttackTime + lastAttack.duration + lastAttack.comboResetTime));
            if (insideComboWindow && !continueGeneralAttackCombo && attackNum < generalAttackCombo.Length - 1)
                attackNum += 1;
            else
                attackNum = 0;
            if (generalAttackCombo[attackNum] != null && generalAttackCombo[attackNum].animTrigger.Length > 0)
                DoAttack(generalAttackCombo[attackNum], UNITSTATE.GENERALATTACK, INPUTACTION.GENERALATTACK);
            return;
        }
        //如果在一次普攻动作进行中又按下了普攻键，则进行普攻连击
        if (action == INPUTACTION.GENERALATTACK && (playerState.currentState == UNITSTATE.GENERALATTACK) && Time.time > (lastAttackTime + lastAttack.duration - 0.1f) && !continueGeneralAttackCombo && isGrounded)
        {
            if (attackNum < generalAttackCombo.Length - 1)
            {
                continueGeneralAttackCombo = true;
                return;
            }
        }
        
    }
    private void DoAttack(DamageObject d, UNITSTATE state, INPUTACTION inputAction)
    {
        animator.SetAnimatorTrigger(d.animTrigger);
        playerState.SetState(state);
        lastAttack = d;
        lastAttack.inflictor = gameObject;
        lastAttackTime = Time.time;
        lastAttackDirection = currentDirection;
        TurnToDir(currentDirection);
        Invoke("Ready", d.duration);
    }
    /// <summary>
    /// 攻击结束，玩家可以进行下一次动作
    /// </summary>
    public void Ready()
    {
        if (continueGeneralAttackCombo)
        {
            continueGeneralAttackCombo = false;
            if (attackNum < generalAttackCombo.Length - 1)
                attackNum += 1;
            else
                attackNum = 0;
            if (generalAttackCombo[attackNum] != null && generalAttackCombo[attackNum].animTrigger.Length > 0)
                DoAttack(generalAttackCombo[attackNum], UNITSTATE.GENERALATTACK, INPUTACTION.GENERALATTACK);
            return;
        }
        playerState.SetState(UNITSTATE.IDLE);
    }
    /// <summary>
    /// set defence on/off
    /// </summary>
    /// <param name="defend"></param>
    private void Defend(bool defend)
    {
        animator.SetAnimatorBool("Defend", defend);
        if (defend)
        {
            //keep turn direction while defending
            if (!canTurnWhileDefending)
            {
                int rot = Mathf.RoundToInt(transform.rotation.eulerAngles.y);
                if (rot >= 180 && rot <= 270)
                    currentDirection = DIRECTION.Left;
                else
                    currentDirection = DIRECTION.Right;
                playerMovement.currentDirection = currentDirection;
            }
            TurnToDir(currentDirection);
            SetVelocity(Vector3.zero);
            playerState.SetState(UNITSTATE.DEFEND);
        }
        else if (playerState.currentState == UNITSTATE.DEFEND)
        {
            playerState.SetState(UNITSTATE.IDLE);
        }
    }
    /// <summary>
    /// 转向
    /// </summary>
    /// <param name="dir"></param>
    public void TurnToDir(DIRECTION dir)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward * -(int)dir);
    }
    private void SetVelocity(Vector3 velocity)
    {
        fixedVelocity = velocity;
        updateVelocity = true;
    }
    //we are hit
    public void Hit(DamageObject d)
    {

    }
    /// <summary>
    /// check if we have hit something (animation event)
    /// </summary>
    public void CheckForHit()
    {
        //Vector3 boxPosition = transform.position + (Vector3.up * lastAttack.collHeight) + Vector3.right * ((int)lastAttackDirection * lastAttack.collDistance);
        //Vector3 boxSize = new Vector3(lastAttack.collSize / 2, lastAttack.collSize / 2, hitZRange / 2);
        //Collider[] hitColliders = Physics.OverlapBox(boxPosition, boxSize, Quaternion.identity, hitLayerMask);

        Vector3 boxPosition = swordHandPos.position;
        float radius = lastAttack.collDistance;
        Collider[] hitColliders = Physics.OverlapSphere(boxPosition, radius, hitLayerMask);
        int i = 0;
        while (i < hitColliders.Length)
        {
            IDamagable<DamageObject> damageObject = hitColliders[i].GetComponent(typeof(IDamagable<DamageObject>)) as IDamagable<DamageObject>;
            if (damageObject != null)
            {
                damageObject.Hit(lastAttack);
                targetHit = true;
            }
            i++;
        }
        if (hitColliders.Length == 0) targetHit = false;

    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (lastAttack != null && Time.time - lastAttackTime < lastAttack.duration)
        {
            Gizmos.color = Color.red;
            //Vector3 boxPosition=transform.position+(Vector3.up*lastAttack.collHeight)+Vector3.right*((int)lastAttackDirection*lastAttack.collDistance);
            //Vector3 boxSize = new Vector3(lastAttack.collSize, lastAttack.collSize, hitZRange);
            Vector3 boxPosition = swordHandPos.position + swordHandPos.forward * 0.5f;
            float radius = lastAttack.collDistance;
            Gizmos.DrawWireSphere(boxPosition, radius);
            //Gizmos.DrawLine(swordHandPos.position,swordHandPos.position + swordHandPos.forward * 0.5f);
        }
    }
#endif
    private void OnEnable()
    {
        InputManager.onInputEvent += MovementInputEvent;
        InputManager.onCombatInputEvent += CombatInputEvent;
    }
    private void OnDisable()
    {
        InputManager.onInputEvent -= MovementInputEvent;
        InputManager.onCombatInputEvent -= CombatInputEvent;
    }

}
